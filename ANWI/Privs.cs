using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Datamodel = ANWI.Database.Model;

namespace ANWI {
	public class Privs {
		#region Instance Variables
		public bool canPromote { get; set; }
		public bool canCertify { get; set; }
		public bool canAssign { get; set; }
		public bool canStartOps { get; set; }
		public bool isFleetAdmin { get; set; }
		#endregion

		#region Constructors
		public Privs() {
			canPromote = false;
			canCertify = false;
			canAssign = false;
			canStartOps = false;
			isFleetAdmin = false;
		}

		private Privs(Datamodel.UserPrivs p) {
			canPromote = p.canPromote;
			canCertify = p.canCertify;
			canAssign = p.canAssign;
			canStartOps = p.canStartOps;
			isFleetAdmin = p.isFleetAdmin;
		}

		public static Privs FetchByUser(int userId) {
			Datamodel.UserPrivs p = null;
			if(Datamodel.UserPrivs.FetchByUser(ref p, userId)) {
				return new Privs(p);
			} else {
				return null;
			}
		}
		#endregion
	}
}
