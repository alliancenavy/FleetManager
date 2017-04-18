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
using System.IO;
using MsgPack.Serialization;
using System.Collections.ObjectModel;

namespace Client {
	/// <summary>
	/// Interaction logic for VesselReg.xaml
	/// </summary>
	public partial class VesselReg : Window {

		private ObservableCollection<VesselRecord> vesselList = new ObservableCollection<VesselRecord>();

		public ObservableCollection<VesselRecord> wpfVesselList { get { return vesselList; } }

		private WebSocket socket = null;

		public VesselReg(WebSocket ws) {
			InitializeComponent();
			socket = ws;
			this.DataContext = this;

			LoadVesselList();
		}

		private void LoadVesselList() {

			using (MemoryStream stream = new MemoryStream()) {
				ANWI.Messaging.Message msg = new ANWI.Messaging.Message(
					ANWI.Messaging.Message.Routing.Target.VesselReg,
					0,
					new ANWI.Messaging.Request(ANWI.Messaging.Request.Type.GetVesselList));
				MessagePackSerializer.Get<ANWI.Messaging.Message>().Pack(stream, msg);
				socket.Send(stream.ToArray());
			}
		}

		public void DeliverMessage(ANWI.Messaging.Message m) {
			if(m.payload is ANWI.Messaging.FullVesselReg) {
				this.Dispatcher.Invoke(() => { vesselList.Clear(); });

				ANWI.Messaging.FullVesselReg fvr = m.payload as ANWI.Messaging.FullVesselReg;

				// Sort the records by ordering
				fvr.vessels.Sort((a, b) => {
					if (a.hull.ordering < b.hull.ordering)
						return -1;
					else if (a.hull.ordering == b.hull.ordering)
						return 0;
					else return 1;
				});

				foreach (Vessel vessel in fvr.vessels) {
					// For now 100 will be the boundary between named and unnamed vessels
					if(vessel.hull.ordering < 100) {
						NamedVessel vr = new NamedVessel();
						vr.v = vessel;
						this.Dispatcher.Invoke(() => { vesselList.Add(vr); });
					} else {
						// TODO
					}
				}

				this.Dispatcher.Invoke(() => {
					Spinner.Visibility = Visibility.Hidden;
				});
			}
		}

		private void Button_NewShip_Click(object sender, RoutedEventArgs e) {

		}
	}
}