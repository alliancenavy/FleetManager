using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebSocketSharp;
using WebSocketSharp.Server;
using Newtonsoft.Json;
using ANWI;
using Datamodel = ANWI.Database.Model;
using Auth0.Core;
using Auth0.Windows;
using Auth0.ManagementApi;
using Auth0.AuthenticationApi;
using MsgPack;
using MsgPack.Serialization;
using System.IO;
using NLog;
using Auth0.AuthenticationApi.Models;

namespace FleetManager.Services {
	public class Auth : WebSocketBehavior {
		private static NLog.Logger logger = LogManager.GetLogger("Auth Service");
		
		private AuthenticationApiClient auth0Client;

		private Version minimumVersion = new Version(0, 1, 0, 0);

		private static string auth0Connection = "Username-Password-Authentication";
		private static string auth0ClientId = "b34x4hALcBeA24rPCcrLW3DZee5b28A0";

		public Auth() {
			auth0Client = new AuthenticationApiClient("stackcollision.auth0.com");
		}

		protected override void OnMessage(MessageEventArgs e) {
			// Deserialize the message
			ANWI.Messaging.Message m = ANWI.Messaging.Message.Receive(e.RawData);

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
					logger.Info($"User {cred.username} has invalid version. Client: {cred.clientVer} Minimum: {minimumVersion}");
					DenyLogin(ANWI.Messaging.LoginResponse.Code.FAILED_VERSION);
					return;
				}

				ResourceOwnerTokenRequest req = new ResourceOwnerTokenRequest() {
					ClientId = auth0ClientId,
					ClientSecret = "mxD7u6wf5Cmrfqt8p244g42vhkl0q9R8qxem80BrHsjqs1XwTjCA7ndy5kL3B9Dd",
					Realm = auth0Connection,
					Username = cred.username,
					Password = cred.password
				};


				AccessTokenResponse token = null;
				try {
					token = await auth0Client.GetTokenAsync(req);
				} catch(Auth0.Core.Exceptions.ApiException e) {
					logger.Error($"Failed to log in user {cred.username}: {e.Message}");
					DenyLogin(ANWI.Messaging.LoginResponse.Code.FAILED_CREDENTIALS);
					return;
				}

				UserInfo user = await auth0Client.GetUserInfoAsync(token.AccessToken);
				
				logger.Info("Successfully authenticated user.  Token: " + token.AccessToken);

				ANWI.AuthenticatedAccount account = new AuthenticatedAccount();
				account.authToken = token.AccessToken;
				account.auth0_id = user.UserId;
				account.nickname = user.NickName;

				// Get the main user profile
				Datamodel.User dbUser = null;
				if(!Datamodel.User.FetchByAuth0(ref dbUser, account.auth0_id)) {
					logger.Info("Profile not found for user " + account.auth0_id +
						". It will be created.");

					// Create a basic profile
					if (!CreateDatabaseUser(user.NickName, user.UserId)) {
						DenyLogin(ANWI.Messaging.LoginResponse.Code.FAILED_SERVER_ERROR);
						return;
					}
				}

				account.profile = Profile.FetchByAuth0(account.auth0_id);

				ANWI.Messaging.Message resp = new ANWI.Messaging.Message(
					ANWI.Messaging.Message.Routing.NoReturn,
					new ANWI.Messaging.LoginResponse(
						ANWI.Messaging.LoginResponse.Code.OK,
						account)
					);

				SendMessage(resp);
			} catch (System.Net.Http.HttpRequestException e) {
				logger.Info("Failed to authenticate account with auth0.");
				DenyLogin(ANWI.Messaging.LoginResponse.Code.FAILED_SERVER_ERROR);
				return;
			}
		}

		private void DenyLogin(ANWI.Messaging.LoginResponse.Code code) {
			ANWI.Messaging.Message resp = new ANWI.Messaging.Message(
				ANWI.Messaging.Message.Routing.NoReturn,
				new ANWI.Messaging.LoginResponse(code, null));

			SendMessage(resp);
		}

		private async void RegisterUser(ANWI.Messaging.RegisterRequest reg) {
			logger.Info($"Registering new user {reg.username}");

			SignupUserRequest req = new SignupUserRequest() {
				ClientId = auth0ClientId,
				Connection = auth0Connection,
				Email = reg.email,
				Password = reg.password
			};

			ANWI.Messaging.RegisterResponse.Code code = ANWI.Messaging.RegisterResponse.Code.OK;

			try {
				SignupUserResponse resp = await auth0Client.SignupUserAsync(req);

				if(!CreateDatabaseUser(reg.username, "auth0|" + resp.Id)) {
					code = ANWI.Messaging.RegisterResponse.Code.FAILED_SERVER_ERROR;
				}
			} catch(Auth0.Core.Exceptions.ApiException e) {
				logger.Error($"Failed to register email {reg.email}: {e.Message}");
				code = ANWI.Messaging.RegisterResponse.Code.FAILED_ALREADY_EXISTS;
			} catch(Exception e) {
				logger.Error($"Other exception caught: {e.Message}");
				code = ANWI.Messaging.RegisterResponse.Code.FAILED_SERVER_ERROR;
			}

			ANWI.Messaging.Message confirm = new ANWI.Messaging.Message(
				ANWI.Messaging.Message.Routing.NoReturn,
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
				logger.Info($"Created user {nickname} ({auth0_id}) in database");
			else
				logger.Error($"Failed to create user {nickname} ({auth0_id}) in database");

			return res;
		}

		private void SendMessage(ANWI.Messaging.Message m) {
			using (MemoryStream stream = new MemoryStream()) {
				MessagePackSerializer.Get<ANWI.Messaging.Message>().Pack(stream, m);
				Send(stream.ToArray());
			}
		}
	}
}
