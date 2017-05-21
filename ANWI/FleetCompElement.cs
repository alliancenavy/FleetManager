using System;
using System.Collections.Generic;
using System.ComponentModel;
using MsgPack.Serialization;

namespace ANWI {

	namespace FleetComp {

		/// <summary>
		/// Base class for all elements which appear in the OOB table
		/// </summary>
		public class FleetUnit : INotifyPropertyChanged {
			public event PropertyChangedEventHandler PropertyChanged;

			public string uuid;

			[MessagePackIgnore]
			public int ordering { get; }

			public FleetUnit(int ordering) {
				this.ordering = ordering;
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
		public class Ship : FleetUnit {
			[MessagePackIgnore]
			private static readonly int ord = 1;

			public LiteVessel v { get; set; }

			private bool _isFlagship;
			public bool isFlagship {
				get { return _isFlagship; }
				set {
					if(_isFlagship != value) {
						_isFlagship = value;
						NotifyPropertyChanged("isFlagship");
					}
				}
			}
			
			public List<OpPosition> positions { get; set; }

			public Ship() : base(ord) {
				v = null;
				isFlagship = false;
				positions = new List<OpPosition>();
			}
		}

		/// <summary>
		/// A wing of small ships
		/// </summary>
		public class Wing : FleetUnit {
			[MessagePackIgnore]
			private static readonly int ord = 2;

			public enum Role {
				INTERCEPTOR,
				CAP,
				BOMBER,
				DROPSHIP
			}

			private string _name;
			public string name {
				get { return _name; }
				set {
					if (_name != value) {
						_name = value;
						NotifyPropertyChanged("name");
					}
				}
			}

			public List<Boat> members { get; set; }

			private Role _primaryRole;
			public Role primaryRole {
				get { return _primaryRole; }
				set {
					if(_primaryRole != value) {
						_primaryRole = value;
						NotifyPropertyChanged("primaryRole");
					}
				}
			}
			public bool isInterceptor {
				get { return _primaryRole == Role.INTERCEPTOR; } }
			public bool isCAP { get { return _primaryRole == Role.CAP; } }
			public bool isBomber { get { return _primaryRole == Role.BOMBER; } }
			public bool isDropship {
				get { return _primaryRole == Role.DROPSHIP; } }

			private string _callsign;
			public string callsign {
				get { return _callsign; }
				set {
					if (_callsign != value) {
						_callsign = value;

						for(int i = 0; i < members.Count; ++i) {
							members[i].callsign = $"{callsign} {i + 1}";
						}

						NotifyPropertyChanged("callsign");
					}
				}
			}

			public string roleIcon { get {
					switch(primaryRole) {
						case Role.INTERCEPTOR:
							return "/images/ops/roleInterceptor.png";
						case Role.CAP:
							return "/images/ops/roleCAP.png";
						case Role.BOMBER:
							return "/images/ops/roleBomber.png";
						case Role.DROPSHIP:
							return "/images/ops/roleDropship.png";
						default:
							return "";
					}
				} }

			public Wing() : base(ord) {
				name = "";
				members = new List<Boat>();
				primaryRole = Role.INTERCEPTOR;
				callsign = "";
			}
		}

		public class Boat : FleetUnit {
			[MessagePackIgnore]
			private static readonly int ord = 3;

			public string wingUUID;
			public Hull type { get; set; }

			private string _callsign;
			public string callsign {
				get { return _callsign; }
				set {
					if(_callsign != value) {
						_callsign = value;
						NotifyPropertyChanged("callsign");
					}
				}
			}

			private bool _isWC;
			public bool isWC {
				get { return _isWC; }
				set {
					if(_isWC != value) {
						_isWC = value;
						NotifyPropertyChanged("isWC");
					}
				}
			}

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

			public Boat() : base(ord) {
				type = null;
				callsign = "";
				isWC = false;
				positions = new List<OpPosition>();
			}
		}
	}
}
