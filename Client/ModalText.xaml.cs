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
	/// Interaction logic for ModalText.xaml
	/// </summary>
	public partial class ModalText : Window {
		public event Action<string> returnText;

		private string originalText = "";

		public ModalText(string title, string initialText) {
			InitializeComponent();
			Text_Entry.Text = initialText;
			originalText = initialText;

			this.Title = title;
		}

		private void Button_OK_Click(object sender, RoutedEventArgs e) {
			returnText(Text_Entry.Text);
			this.Close();
		}

		private void Button_Cancel_Click(object sender, RoutedEventArgs e) {
			returnText(originalText);
			this.Close();
		}
	}
}
