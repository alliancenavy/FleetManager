using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MsgPack.Serialization;

namespace ANWI {
	/// <summary>
	/// A full description of an active operation for transmission
	/// </summary>
	public class Operation {
		#region Instance Members
		public string uuid;

		public string name;
		public OperationType type;
		public OperationStatus status;
		public bool freeMove;
		public bool C2Unified;

		public List<OpParticipant> roster = null;

		[MessagePackKnownCollectionItemType("fu_ship", typeof(FleetComp.Ship))]
		[MessagePackKnownCollectionItemType("fu_wing", typeof(FleetComp.Wing))]
		public List<FleetComp.FleetUnit> fleet = null;
		#endregion
		
		public Operation() {

		}
	}
}
