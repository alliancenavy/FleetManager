using System.Collections.Generic;
using System.ComponentModel;

namespace ANWI {

	namespace FleetComp {

		/// <summary>
		/// Base class for all elements which appear in the OOB table
		/// </summary>
		public class FleetCompElement : INotifyPropertyChanged {
			public event PropertyChangedEventHandler PropertyChanged;

			public string uuid;

			public FleetCompElement() {
				// Empty
			}

			/// <summary>
			/// Notifies the UI when a bound property changes
			/// </summary>
			/// <param name="name"></param>
			public void NotifyPropertyChanged(string name) {
				if (PropertyChanged != null) {
					PropertyChanged(this, new PropertyChangedEventArgs(name));
				}
			}
		}

		/// <summary>
		/// A large named ship
		/// </summary>
		public class NamedShip : FleetCompElement {
			public LiteVessel v { get; set; }
			public bool isFlagship { get; set; }
			
			public List<OpPosition> positions { get; set; }

			public NamedShip() {
				v = null;
				isFlagship = false;
				positions = new List<OpPosition>();
			}
		}

		/// <summary>
		/// A wing of small ships
		/// </summary>
		public class Wing : FleetCompElement {
			public enum Role {
				INTERCEPTOR,
				CAP,
				BOMBER,
				DROPSHIP
			}

			public string name { get; set; }
			public List<WingMember> members { get; set; }
			public Role primaryRole { get; set; }
			public string callsign { get; set; }

			public string roleIcon { get {
					switch(primaryRole) {
						case Role.INTERCEPTOR:
							return "/images/ops/RoleInterceptor.png";
						case Role.CAP:
							return "/images/ops/RoleCAP.png";
						case Role.BOMBER:
							return "/images/ops/RoleBomber.png";
						case Role.DROPSHIP:
							return "/images/ops/RoleDropship.png";
						default:
							return "";
					}
				} }

			public Wing() {
				name = "";
				members = new List<WingMember>();
				primaryRole = Role.INTERCEPTOR;
				callsign = "";
			}
		}

		public class WingMember : FleetCompElement {
			public Hull type { get; set; }
			public string callsign { get; set; }
			public bool isWC { get; set; }
			public List<OpPosition> positions { get; set; }

			public bool isFilled {
				get {
					foreach(OpPosition p in positions) {
						if (p.filledById == -1)
							return false;
					}
					return true;
				}
			}

			public bool hasUnfilledCritical {
				get {
					foreach(OpPosition p in positions) {
						if (p.critical && !p.isFilled)
							return true;
					}
					return false;
				}
			}

			public WingMember() {
				type = null;
				callsign = "";
				isWC = false;
				positions = new List<OpPosition>();
			}
		}
	}
}
