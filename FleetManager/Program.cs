using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebSocketSharp;
using WebSocketSharp.Server;
using Auth0.Windows;
using FleetManager.Services;
using MsgPack.Serialization;
using System.IO;
using NLog;

namespace FleetManager {

	class Program {

		private static NLog.Logger logger = LogManager.GetLogger("Program");

		Dictionary<string, string> CurrentUsers;

		static void Main(string[] args) {
			AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(UEHandler);

			logger.Info("Starting up...");

			Configuration.Load();

			// Open database connection
			if(!ANWI.Database.DBI.Open(Configuration.dbFile)) {
				Console.WriteLine("Failed to open database connection");
				return;
			}

			var wssv = new WebSocketServer(Configuration.fullSocketUrl);
			
			// Set up services
			wssv.AddWebSocketService<Auth>("/auth");
			wssv.AddWebSocketService<Main>("/main");

			// Start the web socket
			wssv.Start();
			Console.ReadKey(true);
			wssv.Stop();
		}

		private static void UEHandler(object sender, UnhandledExceptionEventArgs e) {
			logger.Fatal("Unhandled exception! " + (e.ExceptionObject as Exception));
			logger.Info("Writing crash dump...");
			ANWI.Utility.DumpWriter.MiniDumpToFile("crashdump.dmp");
			Environment.Exit(1);
		}
	}
}
