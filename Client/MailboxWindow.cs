using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ANWI;
using System.Windows;

namespace Client {
	public class MailboxWindow : Window {

		private Dictionary<Type, Action<ANWI.Messaging.IMessagePayload>> msgProcessor =
			new Dictionary<Type, Action<ANWI.Messaging.IMessagePayload>>();

		public MailboxWindow() {
			// Empty
		}

		public void DeliverMessage(ANWI.Messaging.Message msg) {
			msgProcessor[msg.payload.GetType()](msg.payload);
		}

		protected void AddProcessor(Type t, Action<ANWI.Messaging.IMessagePayload> a) {
			msgProcessor.Add(t, a);
		}

	}
}
