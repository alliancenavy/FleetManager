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
				Auth0User user = await auth0.LoginAsync("Username-Password-Authentication",
					cred.username, cred.password);

				logger.Info("Successfully authenticated user.  Token: " + user.Auth0AccessToken);

				ANWI.AuthenticatedAccount account = new AuthenticatedAccount();
				account.authToken = user.Auth0AccessToken;
				account.auth0_id = user.Profile["user_id"].ToString();
				account.nickname = user.Profile["nickname"].ToString();

				// Get the main user profile
				Datamodel.User dbUser = null;
				Datamodel.User.FetchByAuth0(ref dbUser, account.auth0_id);
				dbUser.Acquire();

				// Get their full list of rates
				List<Datamodel.StruckRate> rates = null;
				Datamodel.StruckRate.FetchByUserId(ref rates, dbUser.id);
				foreach(Datamodel.StruckRate r in rates) {
					r.Acquire();
				}

				account.profile = Profile.FromDatamodel(dbUser, rates);

				using (MemoryStream stream = new MemoryStream()) {
					MessagePackSerializer.Get<AuthenticatedAccount>().Pack(stream, account);
					Send(stream.ToArray());
				}
			} catch (System.Net.Http.HttpRequestException e) {
				ANWI.AuthenticatedAccount failed = new AuthenticatedAccount();
				failed.nickname = "";
				failed.auth0_id = "";

				logger.Info("Failed to authenticate account with auth0.");

				using (MemoryStream stream = new MemoryStream()) {
					MessagePackSerializer.Get<AuthenticatedAccount>().Pack(stream, failed);
					Send(stream.ToArray());
				}

				return;
			}
		}
	}
}
