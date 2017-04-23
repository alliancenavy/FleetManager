using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Datamodel = ANWI.Database.Model;
using MsgPack.Serialization;

namespace ANWI {
	public class LiteProfile {
		public int id { get; set; }
		public string nickname { get; set; }
		public Rank rank { get; set; }
		public Rate primaryRate { get; set; }

		[MessagePackIgnore]
		public bool isMe { get; set; } = false;

		public static LiteProfile FromDatamodel(Datamodel.User u, Datamodel.StruckRate r) {
			LiteProfile p = new LiteProfile();

			p.id = u.id;
			p.nickname = u.name;
			p.rank = Rank.FromDatamodel(u.Rank);
			p.primaryRate = Rate.FromDatamodel(r);

			return p;
		}

		public string FullName { get { return rank.abbrev + " " + nickname; } }
	}
}
