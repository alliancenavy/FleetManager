using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.IO;
using NLog;

namespace FleetManager {

	/// <summary>
	/// Loads and stores configuration data for the server
	/// </summary>
	public static class Configuration {

		private static NLog.Logger logger 
			= LogManager.GetLogger("Configuration");

		public class Auth0Settings {
			public string url;
			public string connection;
			public string client;
			public string secret;
		}

		// Settings for Auth0 connection
		public static Auth0Settings auth0Settings { get; private set; }

		// SQLite database file name
		public static string dbFile { get; private set; }

		// Websocket configuration
		public static string socketUrl { get; private set; }
		public static short socketPort { get; private set; }
		public static string fullSocketUrl {
			get { return $"{socketUrl}:{socketPort}"; }
		}

		// Websocket SSL configuration
		public static bool hasSSLConfig { get; private set; }
		public static string sslCertName { get; private set; }
		public static string sslCertPassword { get; private set; }

		// Update service
		public static Version clientVersion { get; private set; }

		/// <summary>
		/// Loads the configuration file
		/// </summary>
		/// <returns></returns>
		public static bool Load() {
			logger.Info("Loading configuration");

			try {
				StreamReader stream = File.OpenText("config.json");
				JsonTextReader reader = new JsonTextReader(stream);
				JObject jsonRoot = (JObject)JToken.ReadFrom(reader);

				JObject auth0Root = (JObject)jsonRoot["auth0"];

				auth0Settings = new Auth0Settings() {
					url = (string)auth0Root["url"],
					connection = (string)auth0Root["connection"],
					client = (string)auth0Root["clientId"],
					secret = (string)auth0Root["secret"]
				};

				dbFile = (string)jsonRoot["dbFile"];

				socketUrl = (string)jsonRoot["socket"]["url"];
				socketPort = (short)jsonRoot["socket"]["port"];

				JObject sslRoot = (JObject)jsonRoot["socket"]["ssl"];
				if (sslRoot != null) {
					hasSSLConfig = true;
					sslCertName = (string)sslRoot["cert"];
					sslCertPassword = (string)sslRoot["password"];
				} else {
					hasSSLConfig = false;
				}

				clientVersion 
					= new Version((string)jsonRoot["update"]["clientVer"]);

			} catch (Exception e) {
				logger.Error("Fatal error loading configuration: " + e);
				return false;
			}

			logger.Info("Done");

			return true;
		}

	}
}
