using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ANWI;
using WebSocketSharp;
using WebSocketSharp.Server;

namespace FleetManager.Services {
	public class Main : WebSocketBehavior {
		public Main() {
			
		}

		protected override void OnMessage(MessageEventArgs e) {
			
		}

		protected override void OnOpen() {
			base.OnOpen();
		}

		protected override void OnClose(CloseEventArgs e) {
			base.OnClose(e);
		}

		protected override void OnError(ErrorEventArgs e) {
			base.OnError(e);
		}
	}
}
