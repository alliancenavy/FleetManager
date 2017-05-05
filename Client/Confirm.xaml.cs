using System;
using System.Windows;

namespace Client {

	/// <summary>
	/// A simple yes/no confirmation window with a variable message
	/// </summary>
	public partial class Confirm : Window {
		
		/// <summary>
		/// Action triggered when yes button is pressed
		/// </summary>
		public event Action yesAction;
		
		/// <summary>
		/// Action triggered when no button is pressed
		/// </summary>
		public event Action noAction;

		/// <summary>
		/// 
		/// </summary>
		/// <param name="message">The message to appear in the window</param>
		public Confirm(string message) {
			InitializeComponent();
			Text_Message.Text = message;
		}

		/// <summary>
		/// Triggers yes action and closes window
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void Button_Yes_Click(object sender, RoutedEventArgs e) {
			this.Close();
			if (yesAction != null)
				yesAction();
		}

		/// <summary>
		/// Triggers no action and closes window
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void Button_No_Click(object sender, RoutedEventArgs e) {
			this.Close();
			if (noAction != null)
				noAction();
		}
	}
}
