using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ANWI {

	/// <summary>
	/// Wrapper around LiteProfile for users participating in an Op
	/// </summary>
	public class OpParticipant {
		public LiteProfile profile { get; set; }
		public bool isFC { get; set; }

		public OpParticipant() {
			profile = null;
			isFC = false;
		}
	}
}
