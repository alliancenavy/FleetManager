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
using ANWI;
using ANWI.Messaging;
using WebSocketSharp;

namespace Client {
	/// <summary>
	/// Interaction logic for ServiceRecord.xaml
	/// </summary>
	public partial class ServiceRecord : Window {

		public class AwardRecord {
			public string Name { get; set; }
			public string Icon { get; set; }
		}

		private Profile profile = null;
		private string auth0_id;
		private WebSocket socket;

		public ServiceRecord(Profile p, string id, WebSocket sock) {
			InitializeComponent();
			this.DataContext = p;
			profile = p;
			auth0_id = id;
			socket = sock;

			// Assigned ship
			if (p.assignedShip == null) {
				Text_CurrentAssignment.Text = "No Current Assignment";
			} else {
				Text_CurrentAssignment.Text = p.assignedShip.name + " (" + p.assignedShip.hull.type
					+ " class " + p.assignedShip.hull.role + ")";
			}
		}

		public void DeliverMessage(Message m) {
			
		}

		private void Button_ChangeName_Click(object sender, RoutedEventArgs e) {
			string newName = profile.nickname;
			ModalText textWindow = new ModalText("Change Name", profile.nickname);
			textWindow.returnText += (t) => { newName = t; };
			textWindow.ShowDialog();

			if (profile.nickname != newName) {
				ANWI.Messaging.Message.Send(
					socket,
					Message.Routing.NoReturn,
					new ChangeNickname(auth0_id, newName)
					);

				profile.nickname = newName;
			}
		}
	}
}
