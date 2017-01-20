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

namespace FleetManager.Services {
	public class Auth : WebSocketBehavior {
		private Auth0Client auth0;

		public Auth() {
			auth0 = new Auth0Client("stackcollision.auth0.com", "b34x4hALcBeA24rPCcrLW3DZee5b28A0");
		}

		protected override void OnMessage(MessageEventArgs e) {
			Console.WriteLine(e.Data);
			
			// Deserialize the message
			ANWI.Credentials cred = JsonConvert.DeserializeObject<ANWI.Credentials>(e.Data);

			// Log in the user
			LoginUser(cred);
		}

		protected override void OnOpen() {
			Console.WriteLine("Connection opened");
		}

		protected override void OnClose(CloseEventArgs e) {
			Console.WriteLine("Connection closed");
		}

		private async void LoginUser(ANWI.Credentials cred) {
			// Authenticate the user with Auth0
			try {
				Auth0User user = await auth0.LoginAsync("Username-Password-Authentication",
					cred.username, cred.password);

				Console.WriteLine("Successfully authenticated user.  Token: " + user.Auth0AccessToken);

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

				using (var stream = new MemoryStream()) {
					MessagePackSerializer.Get<AuthenticatedAccount>().Pack(stream, account);
					Send(stream.ToArray());
				}
			} catch (System.Net.Http.HttpRequestException e) {
				ANWI.AuthenticatedAccount failed = new AuthenticatedAccount();
				failed.nickname = "";
				failed.auth0_id = "";

				Console.WriteLine("Failed to authenticate account with auth0.");

				using (var stream = new MemoryStream()) {
					MessagePackSerializer.Get<AuthenticatedAccount>().Pack(stream, failed);
					Send(stream.ToArray());
				}

				return;
			}
		}
	}
}
