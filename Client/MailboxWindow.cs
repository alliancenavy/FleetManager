using System;
using System.Collections.Generic;
using System.Windows;

namespace Client {

	/// <summary>
	/// An extended version of a WPF Window which supports delivery of ANWI
	/// messages to the proper message handlers.
	/// </summary>
	public class MailboxWindow : Window, IMailbox {

		// Event for when the window closes
		// Allows parent window to remove the record of this window's pointer
		public delegate void OnCloseEventHandler(object sender);
		public event OnCloseEventHandler OnClose;

		// Dictionary of message types to their processors
		private Dictionary<Type, Action<ANWI.Messaging.IMessagePayload>> msgProcessor =
			new Dictionary<Type, Action<ANWI.Messaging.IMessagePayload>>();

		public MailboxWindow() {
			// Empty
		}

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
		protected void AddProcessor(Type t, Action<ANWI.Messaging.IMessagePayload> a) {
			msgProcessor.Add(t, a);
		}

		/// <summary>
		/// Fires the OnClose event
		/// </summary>
		protected void InvokeOnClose() {
			if (OnClose != null)
				OnClose(this);
		}

	}
}
