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
	/// Interaction logic for Confirm.xaml
	/// </summary>
	public partial class Confirm : Window {
		public event Action yesAction;
		public event Action noAction;

		public Confirm(string message) {
			InitializeComponent();
			Text_Message.Text = message;
		}

		private void Button_Yes_Click(object sender, RoutedEventArgs e) {
			this.Close();
			if (yesAction != null)
				yesAction();
		}

		private void Button_No_Click(object sender, RoutedEventArgs e) {
			this.Close();
			if (noAction != null)
				noAction();
		}
	}
}
