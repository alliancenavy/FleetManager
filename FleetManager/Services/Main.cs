using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ANWI;
using WebSocketSharp;
using WebSocketSharp.Server;
using NLog;
using System.IO;
using MsgPack.Serialization;

namespace FleetManager.Services {
	public class Main : WebSocketBehavior {
		private static NLog.Logger logger = LogManager.GetLogger("Main Service");

		public Main() {
			logger.Info("Started");
		}

		protected override void OnMessage(MessageEventArgs e) {
			ANWI.Messaging.Message msg = null;
			using (MemoryStream stream = new MemoryStream(e.RawData)) {
				msg = MessagePackSerializer.Get<ANWI.Messaging.Message>().Unpack(stream);
			}

			logger.Info("Message received. " + msg.payload.ToString());
		}

		protected override void OnOpen() {
			base.OnOpen();
			logger.Info("Connection received");
		}

		protected override void OnClose(CloseEventArgs e) {
			base.OnClose(e);
			logger.Info("Connection closed");
		}

		protected override void OnError(WebSocketSharp.ErrorEventArgs e) {
			base.OnError(e);
		}
	}
}
