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
using Client.VesselRegHelpers;
using WebSocketSharp;
using System.Collections.ObjectModel;

namespace Client {
	/// <summary>
	/// Interaction logic for FleetRegistry.xaml
	/// </summary>
	public partial class FleetRegistry : MailboxWindow {

		private WebSocket socket = null;

		private ObservableCollection<VesselRecord> vesselList = new ObservableCollection<VesselRecord>();
		public ObservableCollection<VesselRecord> wpfVesselList { get { return vesselList; } }

		public FleetRegistry(WebSocket ws) {
			this.DataContext = this;
			InitializeComponent();
			socket = ws;

			base.AddProcessor(typeof(ANWI.Messaging.FullVesselReg), LoadVesselList);

			FetchVesselList();
		}

		private void FetchVesselList() {
			this.Dispatcher.Invoke(() => { Spinner.Visibility = Visibility.Visible; });

			ANWI.Messaging.Message.Send(
				socket,
				ANWI.Messaging.Message.Routing.FleetReg,
				new ANWI.Messaging.Request(ANWI.Messaging.Request.Type.GetFleet));
		}

		private void LoadVesselList(ANWI.Messaging.IMessagePayload m) {
			ANWI.Messaging.FullVesselReg fvr = m as ANWI.Messaging.FullVesselReg;

			fvr.vessels.Sort((a, b) => {
				if (a.hull.ordering < b.hull.ordering)
					return -1;
				else if (a.hull.ordering == b.hull.ordering)
					return 0;
				else return 1;
			});

			foreach(Vessel v in fvr.vessels) {
				// For now 100 will be the boundary between named and unnamed vessels
				if (v.hull.ordering < 100) {
					NamedVessel vr = new NamedVessel();
					vr.v = v;
					this.Dispatcher.Invoke(() => { vesselList.Add(vr); });
				} else {
					// TODO
				}
			}

			this.Dispatcher.Invoke(() => { Spinner.Visibility = Visibility.Hidden; });
		}

		private void Button_NewShip_Click(object sender, RoutedEventArgs e) {

		}

		private void Button_EditShip_Click(object sender, RoutedEventArgs e) {

		}

		private void Button_ViewShip_Click(object sender, RoutedEventArgs e) {

		}

		private void Button_Close_Click(object sender, RoutedEventArgs e) {

		}

		private void Button_RefreshRegistry_Click(object sender, RoutedEventArgs e) {

		}
	}
}
