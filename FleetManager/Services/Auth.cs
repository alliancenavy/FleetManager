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
using Auth0.Windows;
using MsgPack;
using MsgPack.Serialization;
using System.IO;
using NLog;

namespace FleetManager.Services {
	public class Auth : WebSocketBehavior {
		private static NLog.Logger logger = LogManager.GetLogger("Auth Service");

		private Auth0Client auth0;
		private Version minimumVersion = new Version(0, 1, 0, 0);

		public Auth() {
			auth0 = new Auth0Client("stackcollision.auth0.com", "b34x4hALcBeA24rPCcrLW3DZee5b28A0");
		}

		protected override void OnMessage(MessageEventArgs e) {
			// Deserialize the message
			ANWI.Credentials cred;

			using (MemoryStream stream = new MemoryStream(e.RawData)) {
				cred = MessagePackSerializer.Get<Credentials>().Unpack(stream);
			}

			// Log in the user
			LoginUser(cred);
		}

		protected override void OnOpen() {
			logger.Info("Connection opened");
		}

		protected override void OnClose(CloseEventArgs e) {
			logger.Info("Connection closed");
		}

		private async void LoginUser(ANWI.Credentials cred) {
			// Authenticate the user with Auth0
			try {
				// Check version
				if (minimumVersion.CompareTo(cred.clientVersion) > 0) {
					logger.Info($"User {cred.username} has invalid version. Client: {cred.clientVersion} Minimum: {minimumVersion}");
					DenyLogin(ANWI.Messaging.LoginResponse.Code.FAILED_VERSION);
					return;
				}

				// TODO: temporarily removed actual check
				//Auth0User user = await auth0.LoginAsync("Username-Password-Authentication",
				//	cred.username, cred.password);
				Auth0User user = new Auth0User();
				user.Auth0AccessToken = "none";

				logger.Info("Successfully authenticated user.  Token: " + user.Auth0AccessToken);

				ANWI.AuthenticatedAccount account = new AuthenticatedAccount();
				account.authToken = user.Auth0AccessToken;
				account.auth0_id = "auth0|58713654d89baa12399d5000";//user.Profile["user_id"].ToString();
				account.nickname = "Mazer Ludd";//user.Profile["nickname"].ToString();

				// Get the main user profile
				Datamodel.User dbUser = null;
				if(!Datamodel.User.FetchByAuth0(ref dbUser, account.auth0_id)) {
					logger.Info("Profile not found for user " + account.auth0_id +
						". It will be created.");

					// Create a basic profile
					if (!Datamodel.User.Create(ref dbUser,
						"Mazer Ludd",//user.Profile["name"].ToString(),
						"auth0|58713654d89baa12399d5000",//user.Profile["user_id"].ToString(),
						1)) {
						logger.Error("Failed to create profile for new user");

						DenyLogin(ANWI.Messaging.LoginResponse.Code.FAILED_SERVER_ERROR);
						return;
					}
				}

				account.profile = Profile.FetchByAuth0(account.auth0_id);

				ANWI.Messaging.Message resp = new ANWI.Messaging.Message(
					ANWI.Messaging.Message.Routing.NoReturn,
					new ANWI.Messaging.LoginResponse(
						ANWI.Messaging.LoginResponse.Code.SUCCEEDED,
						account)
					);

				using (MemoryStream stream = new MemoryStream()) {
					MessagePackSerializer.Get<ANWI.Messaging.Message>().Pack(stream, resp);
					Send(stream.ToArray());
				}
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

			using (MemoryStream stream = new MemoryStream()) {
				MessagePackSerializer.Get<ANWI.Messaging.Message>().Pack(stream, resp);
				Send(stream.ToArray());
			}
		}
	}
}
