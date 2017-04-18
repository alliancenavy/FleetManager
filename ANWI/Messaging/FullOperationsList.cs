using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ANWI.Messaging {
	public class FullOperationsList : IMessagePayload {
		public List<Operation> ops = null;

		public FullOperationsList() {
			ops = null;
		}

		public FullOperationsList(List<Operation> o) {
			ops = o;
		}

		public override string ToString() {
			return "Type: FullOperationsList. Count: " + ops.Count;
		}
	}
}
