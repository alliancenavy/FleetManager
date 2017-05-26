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

		static void Main(string[] args) {
			AppDomain.CurrentDomain.UnhandledException 
				+= new UnhandledExceptionEventHandler(UEHandler);

			{
				Version v = System.Reflection.Assembly.GetExecutingAssembly().
					GetName().Version;

				Console.Write(
$@"                                         
   _|_|    _|_|_|_|    _|_|      _|_|_|  
 _|    _|  _|        _|    _|  _|        
 _|_|_|_|  _|_|_|    _|    _|    _|_|    
 _|    _|  _|        _|    _|        _|  
 _|    _|  _|          _|_|    _|_|_|    
   Server version {v}                    
                                         
"
					);
			}

			logger.Info("Starting up...");

			Configuration.Load();

			// Open database connection
			if(!ANWI.Database.DBI.Open(Configuration.dbFile)) {
				logger.Fatal("Failed to open database connection");
				Console.ReadKey(true);
				return;
			}

			WebSocketServer wssv = null;
			if (Configuration.hasSSLConfig) {
				logger.Info("Starting WebSocket with SSL");
				wssv = new WebSocketServer(Configuration.socketPort, true);
				wssv.SslConfiguration.ServerCertificate =
					new X509Certificate2(
						Configuration.sslCertName, 
						Configuration.sslCertPassword
						);
			} else {
				logger.Info("Starting WebSocket without SSL");
				wssv = new WebSocketServer(Configuration.fullSocketUrl);
			}


			// Set up services
			wssv.AddWebSocketService<Update>("/update");
			wssv.AddWebSocketService<Auth>("/auth");
			wssv.AddWebSocketService<Main>("/main");
			wssv.AddWebSocketService<Ops>("/ops");

			// Start the web socket
			wssv.Start();

			while (true)
				;
		}

		private static void UEHandler(object sender, 
			UnhandledExceptionEventArgs e) {
			if (e.IsTerminating) {
				Console.WriteLine("Unhandled Exception! " 
					+ (e.ExceptionObject as Exception));
				logger.Info("Writing crash dump...");
				ANWI.Utility.DumpWriter.MiniDumpToFile("crashdump.dmp");
				Console.ReadKey(true);
				Environment.Exit(1);
			} else {
				Console.WriteLine("Unhandled Exception! " 
					+ (e.ExceptionObject as Exception));
			}
		}
	}
}
