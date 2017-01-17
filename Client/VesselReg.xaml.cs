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

namespace Client {
	/// <summary>
	/// Interaction logic for VesselReg.xaml
	/// </summary>
	public partial class VesselReg : Window {

		private List<VesselRecord> vesselList = new List<VesselRecord>();

		public List<VesselRecord> wpfVesselList { get { return vesselList; } }

		public VesselReg() {
			InitializeComponent();
			this.DataContext = this;

			Spinner.Visibility = Visibility.Hidden;
			vesselList.Clear();

			vesselList.Add(new CategoryDivider() {
				text = "Capital"
			});

			vesselList.Add(new NamedVessel() {
				v = new Vessel() {
					name = "ANS Legend of Dave",
					hull = new Hull() {
						role = "Frigate",
						type = "Idris",
						subtype = "-P",
						symbol = "FF"
					},
					owner = "Fleet",
					isLTI = false,
					hullNumber = 10,
					status = Vessel.VesselStatus.ACTIVE
				}
			});

			vesselList.Add(new NamedVessel() {
				v = new Vessel() {
					name = "ANS Suffering is Relative",
					hull = new Hull() {
						role = "Corvette",
						type = "Polaris",
						subtype = "",
						symbol = "K"
					},
					owner = "Meia Cosmos",
					isLTI = true,
					hullNumber = 1,
					status = Vessel.VesselStatus.DESTROYED_WAITING_REPLACEMENT
				}
			});

			vesselList.Add(new NamedVessel() {
				v = new Vessel() {
					name = "ANS Everlasting Snowmew",
					hull = new Hull() {
						role = "Corvette",
						type = "Polaris",
						subtype = "",
						symbol = "K"
					},
					owner = "Spookerton",
					isLTI = true,
					hullNumber = 2,
					status = Vessel.VesselStatus.DRYDOCKED
				}
			});

			vesselList.Add(new CategoryDivider() {
				text = "Sub-Capital"
			});

			vesselList.Add(new NamedVessel() {
				v = new Vessel() {
					name = "ANS Queen Ludd's Revenge",
					hull = new Hull() {
						role = "Cutter",
						type = "Constellation",
						subtype = " Phoenix",
						symbol = "C"
					},
					owner = "Spookerton",
					isLTI = true,
					hullNumber = 5,
					status = Vessel.VesselStatus.ACTIVE
				}
			});

			vesselList.Add(new CategoryDivider() {
				text = "Small Multi-Crew"
			});

			vesselList.Add(new CategoryDivider() {
				text = "Fighters"
			});

			vesselList.Add(new UnnamedVessel() {
				v = new Vessel() {
					hull = new Hull() {
						role = "Fighter",
						type = "Hornet",
						subtype = " F7C",
						manufacturer = "Anvil Aerospace"
					}
				},
				owners = new List<UnnamedVessel.Owner>() {
					new UnnamedVessel.Owner() {name = "Cyanide", LTI = true },
					new UnnamedVessel.Owner() {name = "Nadav", LTI = true }
				}
			});

			vesselList.Add(new UnnamedVessel() {
				v = new Vessel() {
					hull = new Hull() {
						role = "Interceptor",
						type = "Gladius",
						subtype = "",
						manufacturer = "AEGIS Dynamics"
					}
				},
				owners = new List<UnnamedVessel.Owner>() {
					new UnnamedVessel.Owner() {name = "Mazer Ludd", LTI = true },
					new UnnamedVessel.Owner() {name = "Syxx", LTI = true }
				}
			});
		}
	}
}
