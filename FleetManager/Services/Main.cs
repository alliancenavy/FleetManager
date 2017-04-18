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

		private Dictionary<Type, Func<ANWI.Messaging.IMessagePayload, 
			ANWI.Messaging.IMessagePayload>> msgProcessors = null;

		public Main() {
			logger.Info("Started");

			// Build the message processor dictionary
			msgProcessors = new Dictionary<Type, Func<ANWI.Messaging.IMessagePayload, 
				ANWI.Messaging.IMessagePayload>>() {
				{ typeof(ANWI.Messaging.Request), ProcessRequestMessage },
				{ typeof(ANWI.Messaging.ChangeNickname), ProcessChangeNickname }
			};
		}

		protected override void OnMessage(MessageEventArgs e) {
			ANWI.Messaging.Message msg = ANWI.Messaging.Message.Receive(e.RawData);

			logger.Info("Message received. " + msg.payload.ToString());
			logger.Info(BitConverter.ToString(e.RawData));

			ANWI.Messaging.IMessagePayload p = 
				msgProcessors[msg.payload.GetType()](msg.payload);

			ANWI.Messaging.Message response = new ANWI.Messaging.Message(msg.address, p);

			if (response != null) {
				using (MemoryStream stream = new MemoryStream()) {
					MessagePackSerializer.Get<ANWI.Messaging.Message>().Pack(stream, response);
					Send(stream.ToArray());
				}
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

		private ANWI.Messaging.IMessagePayload ProcessRequestMessage(ANWI.Messaging.IMessagePayload p) {
			ANWI.Messaging.Request req = p as ANWI.Messaging.Request;
			switch(req.type) {
				case ANWI.Messaging.Request.Type.GetFleet: {
						List<Datamodel.UserShip> all = null;
						Datamodel.UserShip.FetchNotDestroyed(ref all);

						List<Vessel> vessels = new List<Vessel>();

						foreach(Datamodel.UserShip dmship in all) {
							vessels.Add(Vessel.FromDatamodel(dmship));
						}

						return new ANWI.Messaging.FullVesselReg(vessels);
					}

				case ANWI.Messaging.Request.Type.GetOperations: {
						List<Operation> ops = new List<Operation>();

						ops.Add(new Operation() {
							name = "Daily Homeguard",
							type = Operation.Type.PATROL,
							status = Operation.Status.ACTIVE,
							currentMembers = 6,
							neededMembers = 5,
							totalSlots = 8,
							id = 1238456
						});

						ops.Add(new Operation() {
							name = "Attack Station",
							type = Operation.Type.ASSAULT,
							status = Operation.Status.STAGING,
							currentMembers = 3,
							neededMembers = 10,
							totalSlots = 20,
							id = 1203913
						});

						ops.Add(new Operation() {
							name = "Unrep ANS Legend of Dave",
							type = Operation.Type.LOGISTICS,
							status = Operation.Status.DISMISSING,
							currentMembers = 2,
							neededMembers = 2,
							totalSlots = 3,
							id = 2357345
						});

						return new ANWI.Messaging.FullOperationsList(ops);
					}
			}

			return null;
		}

		private ANWI.Messaging.IMessagePayload ProcessChangeNickname(ANWI.Messaging.IMessagePayload p) {
			ANWI.Messaging.ChangeNickname cn = p as ANWI.Messaging.ChangeNickname;

			Datamodel.User user = null;
			if(!Datamodel.User.FetchByAuth0(ref user, cn.auth0_id)) {
				logger.Error("Failed to change name.  Could not select user.");
				return null;
			}

			user.name = cn.newName;
			if(!Datamodel.User.Store(user)) {
				logger.Error("Failed to update name is database.");
				return null;
			}

			logger.Info("Name successfully changed.");
			return null;
		}
	}
}
