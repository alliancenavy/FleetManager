using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

		public List<OpParticipant> roster = null;

		public List<FleetComp.FleetUnit> fleet = null;
		#endregion
		
		public Operation() {

		}
	}
}
