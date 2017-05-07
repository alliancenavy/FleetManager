using ANWI;
using System.Windows.Controls;
using System.Windows;

namespace Client {

	namespace FleetComp {

		/// <summary>
		/// Base class for all elements which appear in the OOB table
		/// </summary>
		public class FleetCompElement {
			OpParticipant assigned { get; set; } = null;
		}

		/// <summary>
		/// A large named ship
		/// </summary>
		public class NamedShip : FleetCompElement {
			public LiteVessel v { get; set; }
			public bool isFlagship { get; set; }
		}

		/// <summary>
		/// A wing of small ships
		/// </summary>
		public class Wing : FleetCompElement {

		}
	}
}
