using ANWI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
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

namespace Client.Operations {
	/// <summary>
	/// Interaction logic for AddFleetShip.xaml
	/// </summary>
	public partial class AddFleetShip : MailboxWindow, INotifyPropertyChanged {

		public event PropertyChangedEventHandler PropertyChanged;

		private ObservableCollection<LiteVessel> _vesselList
			= new ObservableCollection<LiteVessel>();
		public ObservableCollection<LiteVessel> vesselList {
			get { return _vesselList; }
		}

		public event Action<int> returnNewShip;

		public AddFleetShip() {
			this.DataContext = this;
			InitializeComponent();

			this.AddProcessor(typeof(ANWI.Messaging.FullVesselReg),
				ProcessVesselList);

			MessageRouter.Instance.SendMain(
				new ANWI.Messaging.Request(
					ANWI.Messaging.Request.Type.GetAvailableFleet),
				this
				);
		}

		private void ProcessVesselList(ANWI.Messaging.IMessagePayload p) {
			ANWI.Messaging.FullVesselReg reg 
				= p as ANWI.Messaging.FullVesselReg;

			_vesselList = new ObservableCollection<LiteVessel>(reg.vessels);
			NotifyPropertyChanged("vesselList");
		}

		private void Button_OK_Click(object sender, RoutedEventArgs e) {
			if(returnNewShip != null && Combobox_Ships.SelectedItem != null) {
				LiteVessel v = Combobox_Ships.SelectedItem as LiteVessel;
				returnNewShip(v.id);
				this.Close();
			}
		}

		private void Button_Cancel_Click(object sender, RoutedEventArgs e) {
			this.Close();
		}

		/// <summary>
		/// Notifies the UI that a bound property has changed
		/// </summary>
		/// <param name="name"></param>
		public void NotifyPropertyChanged(string name) {
			if (PropertyChanged != null) {
				PropertyChanged(this, new PropertyChangedEventArgs(name));
			}
		}
	}
}
