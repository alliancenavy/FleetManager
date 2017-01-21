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

namespace FleetManager {

	class Program {

		Dictionary<string, string> CurrentUsers;

		static void Main(string[] args) {
			// Open database connection
			if(!ANWI.Database.DBI.Open()) {
				Console.WriteLine("Failed to open database connection");
				return;
			}

			var wssv = new WebSocketServer("ws://localhost:9000");
			
			// Set up services
			wssv.AddWebSocketService<Auth>("/auth");
			wssv.AddWebSocketService<Main>("/main");

			// Start the web socket
			wssv.Start();
			Console.ReadKey(true);
			wssv.Stop();
		}
	}
}
