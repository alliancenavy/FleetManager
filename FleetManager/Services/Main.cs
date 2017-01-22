using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ANWI;
using Datamodel = ANWI.Database.Model;
using WebSocketSharp;
using WebSocketSharp.Server;
using NLog;
using System.IO;
using MsgPack.Serialization;

namespace FleetManager.Services {
	public class Main : WebSocketBehavior {
		private static NLog.Logger logger = LogManager.GetLogger("Main Service");

		private Dictionary<Type, Func<ANWI.Messaging.MessagePayload, 
			ANWI.Messaging.MessagePayload>> msgProcessors = null;

		public Main() {
			logger.Info("Started");

			// Build the message processor dictionary
			msgProcessors = new Dictionary<Type, Func<ANWI.Messaging.MessagePayload, 
				ANWI.Messaging.MessagePayload>>() {
				{ typeof(ANWI.Messaging.Request), ProcessRequestMessage }
			};
		}

		protected override void OnMessage(MessageEventArgs e) {
			ANWI.Messaging.Message msg = null;
			using (MemoryStream stream = new MemoryStream(e.RawData)) {
				msg = MessagePackSerializer.Get<ANWI.Messaging.Message>().Unpack(stream);
			}

			logger.Info("Message received. " + msg.payload.ToString());

			ANWI.Messaging.MessagePayload p = 
				msgProcessors[msg.payload.GetType()](msg.payload);

			ANWI.Messaging.Message response = new ANWI.Messaging.Message(msg.address, p);

			using(MemoryStream stream = new MemoryStream()) {
				MessagePackSerializer.Get<ANWI.Messaging.Message>().Pack(stream, response);
				Send(stream.ToArray());
			}
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

		private ANWI.Messaging.MessagePayload ProcessRequestMessage(ANWI.Messaging.MessagePayload p) {
			ANWI.Messaging.Request req = p as ANWI.Messaging.Request;
			switch(req.type) {
				case ANWI.Messaging.Request.Type.GetVesselList: {
						List<Datamodel.UserShip> all = null;
						Datamodel.UserShip.FetchNotDestroyed(ref all);

						List<Vessel> vessels = new List<Vessel>();

						foreach(Datamodel.UserShip dmship in all) {
							vessels.Add(Vessel.FromDatamodel(dmship));
						}

						return new ANWI.Messaging.FullVesselReg(vessels);
					}
					break;
			}

			return null;
		}
	}
}
