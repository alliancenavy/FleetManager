using System;
using WebSocketSharp;
using WebSocketSharp.Server;
using ANWI;
using Datamodel = ANWI.Database.Model;
using Auth0.AuthenticationApi;
using MsgPack.Serialization;
using System.IO;
using NLog;
using Auth0.AuthenticationApi.Models;

namespace FleetManager.Services {
	public class Auth : WebSocketBehavior {
		private static NLog.Logger logger 
			= LogManager.GetLogger("Auth Service");
		
		private AuthenticationApiClient auth0Client;

		// Minimum version the client must be running to connect
		private Version minimumVersion = new Version(0, 2, 0, 0);

		public Auth() {
			// TODO: Mode this URL to config
			auth0Client 
				= new AuthenticationApiClient(Configuration.auth0Settings.url);
		}

		protected override void OnMessage(MessageEventArgs e) {
			// Deserialize the message
			ANWI.Messaging.Message m 
				= ANWI.Messaging.Message.Receive(e.RawData);

			if (m.payload is ANWI.Messaging.LoginRequest) {
				LoginUser(m.payload as ANWI.Messaging.LoginRequest);
			} else if(m.payload is ANWI.Messaging.RegisterRequest) {
				RegisterUser(m.payload as ANWI.Messaging.RegisterRequest);
			}
		}

		protected override void OnOpen() {
			logger.Info("Connection opened");
		}

		protected override void OnClose(CloseEventArgs e) {
			logger.Info("Connection closed");
		}

		private async void LoginUser(ANWI.Messaging.LoginRequest cred) {
			// Authenticate the user with Auth0
			try {
				// Check version
				if (minimumVersion.CompareTo(cred.clientVer) > 0) {
					logger.Info(
						$"User {cred.username} has invalid version. " +
						$"Client: {cred.clientVer} Minimum: {minimumVersion}");
					DenyLogin(ANWI.Messaging.LoginResponse.Code.FAILED_VERSION);
					return;
				}

				ResourceOwnerTokenRequest req 
					= new ResourceOwnerTokenRequest() {
					ClientId = Configuration.auth0Settings.client,
					ClientSecret = Configuration.auth0Settings.secret,
					Realm = Configuration.auth0Settings.connection,
					Username = cred.username,
					Password = cred.password
				};


				AccessTokenResponse token = null;
				try {
					token = await auth0Client.GetTokenAsync(req);
				} catch(Auth0.Core.Exceptions.ApiException e) {
					logger.Error(
						$"Failed to log in user {cred.username}: {e.Message}");
					DenyLogin(
						ANWI.Messaging.LoginResponse.Code.FAILED_CREDENTIALS);
					return;
				}

				UserInfo user 
					= await auth0Client.GetUserInfoAsync(token.AccessToken);
				
				logger.Info("Successfully authenticated user.  Token: " + 
					token.AccessToken);

				ANWI.AuthenticatedAccount account = new AuthenticatedAccount();
				account.authToken = token.AccessToken;
				account.auth0_id = user.UserId;
				account.nickname = user.NickName;

				// Get the main user profile
				Datamodel.User dbUser = null;
				if(!Datamodel.User.FetchByAuth0(ref dbUser, account.auth0_id)) {
					logger.Info("Profile not found for user " + 
						account.auth0_id + ". It will be created.");

					// Create a basic profile
					if (!CreateDatabaseUser(user.NickName, user.UserId)) {
						DenyLogin(ANWI.Messaging.LoginResponse.
							Code.FAILED_SERVER_ERROR);
						return;
					}
				}

				account.profile = Profile.FetchByAuth0(account.auth0_id);

				ANWI.Messaging.Message resp = new ANWI.Messaging.Message(
					0,
					new ANWI.Messaging.LoginResponse(
						ANWI.Messaging.LoginResponse.Code.OK,
						account)
					);

				SendMessage(resp);
			} catch (System.Net.Http.HttpRequestException e) {
				logger.Info("Failed to authenticate account with auth0.");
				DenyLogin(
					ANWI.Messaging.LoginResponse.Code.FAILED_SERVER_ERROR);
				return;
			}
		}

		private void DenyLogin(ANWI.Messaging.LoginResponse.Code code) {
			ANWI.Messaging.Message resp = new ANWI.Messaging.Message(
				0,
				new ANWI.Messaging.LoginResponse(code, null));

			SendMessage(resp);
		}

		private async void RegisterUser(ANWI.Messaging.RegisterRequest reg) {
			logger.Info($"Registering new user {reg.username}");

			SignupUserRequest req = new SignupUserRequest() {
				ClientId = Configuration.auth0Settings.client,
				Connection = Configuration.auth0Settings.connection,
				Email = reg.email,
				Password = reg.password
			};

			ANWI.Messaging.RegisterResponse.Code code 
				= ANWI.Messaging.RegisterResponse.Code.OK;

			try {
				SignupUserResponse resp 
					= await auth0Client.SignupUserAsync(req);

				if(!CreateDatabaseUser(reg.username, "auth0|" + resp.Id)) {
					code = ANWI.Messaging.RegisterResponse.
						Code.FAILED_SERVER_ERROR;
				}
			} catch(Auth0.Core.Exceptions.ApiException e) {
				logger.Error(
					$"Failed to register email {reg.email}: {e.Message}");
				code = ANWI.Messaging.RegisterResponse.
					Code.FAILED_ALREADY_EXISTS;
			} catch(Exception e) {
				logger.Error($"Other exception caught: {e.Message}");
				code = ANWI.Messaging.RegisterResponse.Code.FAILED_SERVER_ERROR;
			}

			ANWI.Messaging.Message confirm = new ANWI.Messaging.Message(
				0,
				new ANWI.Messaging.RegisterResponse(code));

			SendMessage(confirm);
			
		}

		private bool CreateDatabaseUser(string nickname, string auth0_id) {
			// Create a basic profile
			Datamodel.User dbUser = null;
			bool res = Datamodel.User.Create(ref dbUser,
				nickname,
				auth0_id,
				1);

			if (res)
				logger.Info(
					$"Created user {nickname} ({auth0_id}) in database");
			else
				logger.Error(
					$"Failed to create user {nickname} ({auth0_id}) in database"
					);

			return res;
		}

		private void SendMessage(ANWI.Messaging.Message m) {
			using (MemoryStream stream = new MemoryStream()) {
				MessagePackSerializer.Get<ANWI.Messaging.Message>().Pack(
					stream, m);
				Send(stream.ToArray());
			}
		}
	}
}
