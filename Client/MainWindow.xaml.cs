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
using System.Windows.Navigation;
using System.Windows.Shapes;
using WebSocketSharp;
using ANWI;

namespace Client {
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window {

		private readonly WebSocket socket;
		private AuthenticatedAccount account = null;

		/// <summary>
		/// Form constructor
		/// Opens a modal login window first.  When control returns it checks if there's
		/// a valid user and populates their profile info.
		/// </summary>
		public MainWindow() {
			InitializeComponent();
			
			// Open a modal login window
			// When the window closes the authclient member will be either null
			// or have a structure.
			// If null, the login failed.
			Login logwin = new Client.Login();
			logwin.returnuser += value => account = value;
			logwin.ShowDialog();

			// Once the control returns check if we have a valid account
			if(account == null) {
				// Just quit the app
				Application.Current.Shutdown();
			}

			// TODO: populate profile info
		}
	}
}
