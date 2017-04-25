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
using System.ComponentModel;

namespace Client {
	/// <summary>
	/// Interaction logic for FleetRegistry.xaml
	/// </summary>
	public partial class FleetRegistry : MailboxWindow, INotifyPropertyChanged {

		public event PropertyChangedEventHandler PropertyChanged;

		private WebSocket socket = null;

		private ObservableCollection<VesselRecord> vesselList = new ObservableCollection<VesselRecord>();
		public ObservableCollection<VesselRecord> wpfVesselList { get { return vesselList; } }

		private NamedVessel currentVessel = null;
		public NamedVessel wpfCurrentVessel {
			get { return currentVessel; }
			set {
				if(currentVessel != value) {
					currentVessel = value;
					NotifyPropertyChanged("wpfCurrentVessel");
				}
			}
		}

		public FleetRegistry(WebSocket ws) {
			this.DataContext = this;
			InitializeComponent();
			socket = ws;

			base.AddProcessor(typeof(ANWI.Messaging.FullVesselReg), LoadVesselList);
			base.AddProcessor(typeof(ANWI.Messaging.FullVesselDetails), LoadVesselDetail);

			FetchVesselList();
		}

		private void FetchVesselList() {
			this.Dispatcher.Invoke(() => { Spinner_List.Visibility = Visibility.Visible; });

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

			this.Dispatcher.Invoke(() => { Spinner_List.Visibility = Visibility.Hidden; });
		}

		private void FetchVesselDetail(int id) {
			this.Dispatcher.Invoke(() => {
				Spinner_Detail.Visibility = Visibility.Visible;
				Button_ViewShip.IsEnabled = false;
			});

			ANWI.Messaging.Message.Send(
				socket,
				ANWI.Messaging.Message.Routing.FleetReg,
				new ANWI.Messaging.Request(ANWI.Messaging.Request.Type.GetShipDetail, id));
		}

		private void LoadVesselDetail(ANWI.Messaging.IMessagePayload m) {
			this.Dispatcher.Invoke(() => {
				Spinner_Detail.Visibility = Visibility.Hidden;
				Button_ViewShip.IsEnabled = true;
			});

			ANWI.Messaging.FullVesselDetails fvd = m as ANWI.Messaging.FullVesselDetails;
			currentVessel.details = fvd.details;
			NotifyPropertyChanged("wpfCurrentVessel");
		}

		private void Button_NewShip_Click(object sender, RoutedEventArgs e) {

		}

		private void Button_ViewShip_Click(object sender, RoutedEventArgs e) {
			if(List_Fleet.SelectedItem != null) {
				VesselRecord vr = List_Fleet.SelectedItem as VesselRecord;
				if (vr is VesselRegHelpers.NamedVessel) {
					wpfCurrentVessel = vr as NamedVessel;
					FetchVesselDetail(currentVessel.id);
				}
			}
		}

		private void Button_Close_Click(object sender, RoutedEventArgs e) {

		}

		private void Button_RefreshRegistry_Click(object sender, RoutedEventArgs e) {

		}

		public void NotifyPropertyChanged(string name) {
			if (PropertyChanged != null) {
				PropertyChanged(this, new PropertyChangedEventArgs(name));
			}
		}
	}
}
