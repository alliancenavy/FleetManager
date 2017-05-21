using ANWI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client {
	namespace C2 {

		public class Channel {
			public string unitUUID;
			public bool expanded { get; set; } = true;
			public string name { get; set; }
			public List<User> users { get; set; } = new List<User>();
		}

		public class User {
			public bool thisUser { get; set; } = false;
			public bool commander { get; set; } = false;
			public string rank { get; set; }
			public string name { get; set; }

			public string fullName { get { return $"{rank} {name}"; } }

			public string icon { get {
					if (commander)
						return "images/ops/ts_commander.png";
					else
						return "images/ops/ts_member.png";
				} }

			public User(OpParticipant member) {
				commander = member.position.role.channelCdr;
				rank = member.profile.rank.abbrev;
				name = member.profile.nickname;
			}
		}

	}
}
