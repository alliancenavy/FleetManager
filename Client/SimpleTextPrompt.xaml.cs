using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Client {
	/// <summary>
	/// Simple text entry prompt
	/// </summary>
	public partial class SimpleTextPrompt : Window {

		// Subscribe to receive entered text
		public event Action<string> ReturnText;

		public SimpleTextPrompt(string title, string initialText = "") {
			InitializeComponent();

			this.Title = title;
			Textbox_Entry.Text = initialText;
		}

		private void Accept(object sender, RoutedEventArgs e) {
			if(Textbox_Entry.Text != "") {
				if(ReturnText != null)
					ReturnText(Textbox_Entry.Text);
				this.Close();
			}
		}

		private void Button_Cancel_Click(object sender, RoutedEventArgs e) {
			this.Close();
		}

		private void Textbox_Entry_KeyDown(object sender, KeyEventArgs e) {
			if (e.Key == Key.Enter)
				Accept(e, null);
		}
	}
}
