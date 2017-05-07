using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ANWI.Utility {
	
	/// <summary>
	/// Supports delivery of ANWI Messages to the proper handlers
	/// Inherit from this class.
	/// </summary>
	public class Mailbox {
		// Dictionary of message types to their processors
		private Dictionary<Type, Action<ANWI.Messaging.IMessagePayload>> 
			msgProcessor =
			new Dictionary<Type, Action<ANWI.Messaging.IMessagePayload>>();

		/// <summary>
		/// Deliver a message to this window.  Passess it on to the correct
		/// processor function.
		/// </summary>
		/// <param name="msg"></param>
		public void DeliverMessage(ANWI.Messaging.Message msg) {
			msgProcessor[msg.payload.GetType()](msg.payload);
		}

		/// <summary>
		/// Inheriting classes can use this to add a processor function for a
		/// given message type.
		/// </summary>
		/// <param name="t"></param>
		/// <param name="a"></param>
		protected void 
		AddProcessor(Type t, Action<ANWI.Messaging.IMessagePayload> a) {
			msgProcessor.Add(t, a);
		}
	}
}
