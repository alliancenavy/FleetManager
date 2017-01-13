using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebSocketSharp;
using WebSocketSharp.Server;
using Newtonsoft.Json;
using ANWI;
using Auth0.Windows;

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

				Console.WriteLine("Successfully authenticated user.");

				ANWI.AuthenticatedAccount account = new AuthenticatedAccount();
				account.idToken = user.IdToken;
				account.nickname = user.Profile["nickname"].ToString();

				// TODO: This is placeholder account info
				account.profile.nickname = "Mazer Ludd";
				account.profile.rank.name = "Captain";
				account.profile.rank.abbrev = "CAPT";
				account.profile.assignedShip.name = "ANS This Isn't Over";
				account.profile.assignedShip.hull.type = "Polaris";
				account.profile.assignedShip.hull.role = "Corvette";
				account.profile.rates.Add(new Rate() {
					id = 1,
					name = "Fighter Pilot",
					abbrev = "FP",
					rank = 1
				});
				account.profile.primaryRate = 0;

				Send(JsonConvert.SerializeObject(account));
			} catch (System.Net.Http.HttpRequestException e) {
				ANWI.AuthenticatedAccount failed = new AuthenticatedAccount();
				failed.nickname = "";
				failed.idToken = "";

				Console.WriteLine("Failed to authenticate account with auth0.");

				Send(JsonConvert.SerializeObject(failed));
				return;
			}
		}
	}
}
