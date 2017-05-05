using System;
using System.Collections.Generic;
using WebSocketSharp.Server;
using FleetManager.Services;
using NLog;
using System.Security.Cryptography.X509Certificates;

namespace FleetManager {

	/// <summary>
	/// Server entry point
	/// </summary>
	class Program {

		private static NLog.Logger logger = LogManager.GetLogger("Program");

		Dictionary<string, string> CurrentUsers;

		static void Main(string[] args) {
			AppDomain.CurrentDomain.UnhandledException 
				+= new UnhandledExceptionEventHandler(UEHandler);

			logger.Info("Starting up...");

			Configuration.Load();

			// Open database connection
			if(!ANWI.Database.DBI.Open(Configuration.dbFile)) {
				Console.WriteLine("Failed to open database connection");
				return;
			}

			WebSocketServer wssv = null;
			if (Configuration.hasSSLConfig) {
				wssv = new WebSocketServer(Configuration.socketPort, true);
				wssv.SslConfiguration.ServerCertificate =
					new X509Certificate2(
						Configuration.sslCertName, 
						Configuration.sslCertPassword
						);
			} else {
				wssv = new WebSocketServer(Configuration.fullSocketUrl);
			}

			
			// Set up services
			wssv.AddWebSocketService<Auth>("/auth");
			wssv.AddWebSocketService<Main>("/main");

			// Start the web socket
			wssv.Start();
			Console.ReadKey(true);
			wssv.Stop();
		}

		private static void UEHandler(object sender, 
			UnhandledExceptionEventArgs e) {
			if (e.IsTerminating) {
				logger.Fatal("Unhandled Exception! " 
					+ (e.ExceptionObject as Exception));
				logger.Info("Writing crash dump...");
				ANWI.Utility.DumpWriter.MiniDumpToFile("crashdump.dmp");
				Environment.Exit(1);
			} else {
				logger.Error("Unhandled Exception! " 
					+ (e.ExceptionObject as Exception));
			}
		}
	}
}
