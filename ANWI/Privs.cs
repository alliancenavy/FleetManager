using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Datamodel = ANWI.Database.Model;

namespace ANWI {
	public class Privs {
		public bool canPromote { get; set; }
		public bool canCertify { get; set; }

		public Privs() {
			canPromote = false;
			canCertify = false;
		}

		public Privs(bool promote, bool certify) {
			canPromote = promote;
			canCertify = certify;
		}

		public static Privs FromDatamodel(Datamodel.UserPrivs p) {
			Privs output = new Privs();

			output.canPromote = p.canPromote;
			output.canCertify = p.canCertify;

			return output;
		}
	}
}
