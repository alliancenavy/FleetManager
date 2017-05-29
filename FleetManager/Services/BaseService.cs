using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebSocketSharp.Server;
using ANWI;
using NLog;
using WebSocketSharp;
using System.IO;
using MsgPack.Serialization;

namespace FleetManager.Services {

	/// <summary>
	/// Provides common functionality for all services
	/// </summary>
	public abstract class BaseService : WebSocketBehavior {

		protected NLog.Logger logger = null;

		private ConnectedUser myUser = null;

		private Dictionary<Type, Func<ANWI.Messaging.IMessagePayload,
			ANWI.Messaging.IMessagePayload>> msgProcessors
			= new Dictionary<Type, Func<ANWI.Messaging.IMessagePayload, 
				ANWI.Messaging.IMessagePayload>>();
		
		protected abstract string GetLogIdentifier();

		protected BaseService(string name) {
			logger = LogManager.GetLogger(name);
		}

		/// <summary>
		/// Adds a message processor function which will be automatically
		/// called when the appropriate message comes in.
		/// </summary>
		/// <param name="t"></param>
		/// <param name="processor"></param>
		protected void
		AddProcessor(Type t, Func<ANWI.Messaging.IMessagePayload, 
			ANWI.Messaging.IMessagePayload> processor) {
			msgProcessors.Add(t, processor);
		}

		/// <summary>
		/// Returns the user connected to this instance
		/// </summary>
		/// <param name="token"></param>
		/// <returns></returns>
		protected ConnectedUser GetUser() {
			return myUser;
		}

		/// <summary>
		/// New connection starting point
		/// </summary>
		protected override void OnOpen() {
			base.OnOpen();

			myUser = new ConnectedUser(this.Context);

			logger.Info($"Connection received from {GetLogIdentifier()}");
		}

		/// <summary>
		/// Connection ending point
		/// </summary>
		/// <param name="e"></param>
		protected override void OnClose(CloseEventArgs e) {
			base.OnClose(e);
			logger.Info($"Connection from {GetLogIdentifier()} closed");
		}

		/// <summary>
		/// Socket error handler
		/// </summary>
		/// <param name="e"></param>
		protected override void OnError(WebSocketSharp.ErrorEventArgs e) {
			base.OnError(e);
			logger.Error($"Error in connection from {GetLogIdentifier()}: {e}");
		}

		/// <summary>
		/// Message router.  Sends messages to correct processor function
		/// </summary>
		/// <param name="e"></param>
		protected sealed override void OnMessage(MessageEventArgs e) {
			ANWI.Messaging.Message msg
				= ANWI.Messaging.Message.Receive(e.RawData);

			logger.Info($"Message received #{msg.sequence} from " +
				$"{GetLogIdentifier()}. {msg.payload.ToString()}");

			Func<ANWI.Messaging.IMessagePayload, ANWI.Messaging.IMessagePayload>
				processor = msgProcessors[msg.payload.GetType()];
			if(processor != null) {
				ANWI.Messaging.IMessagePayload p = processor(msg.payload);

				if (p != null) {
					ANWI.Messaging.Message response
						= new ANWI.Messaging.Message(msg.sequence, p);
					Send(response);
				}

			} else {
				logger.Error("No message processor found for payload type " +
					msg.payload.GetType());
			}
		}

		/// <summary>
		/// Serializes an ANWI message and sends it to the current context
		/// </summary>
		/// <param name="msg"></param>
		protected void Send(ANWI.Messaging.Message msg) {
			MemoryStream stream = new MemoryStream();
			MessagePackSerializer.Get<ANWI.Messaging.Message>().Pack(
				stream, msg);
			Send(stream.ToArray());
		}
	}
}
