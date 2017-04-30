using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ANWI.Messaging {
	public class ConfirmUpdate : IMessagePayload {
		public bool success;
		public int updatedId;

		public ConfirmUpdate() {
			success = false;
			updatedId = 0;
		}

		public ConfirmUpdate(bool s, int id) {
			success = s;
			updatedId = id;
		}

		public override string ToString() {
			return $"Type: ConfirmUpdate.  Success: {success} Id: {updatedId}";
		}
	}
}
