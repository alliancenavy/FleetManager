using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebSocketSharp;
using WebSocketSharp.Server;
using Auth0.Windows;
using FleetManager.Services;

namespace FleetManager {

	class Program {
		static void Main(string[] args) {
			var wssv = new WebSocketServer("ws://localhost:9000");
			
			// Set up services
			wssv.AddWebSocketService<Auth>("/auth");

			// Start the web socket
			wssv.Start();
			Console.ReadKey(true);
			wssv.Stop();
		}
	}
}
