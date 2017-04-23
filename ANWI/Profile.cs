using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Datamodel = ANWI.Database.Model;
using MsgPack.Serialization;

namespace ANWI {
	/// <summary>
	/// A user's profile.
	/// Incudings things like name, rank, rates, etc
	/// </summary>
	public class Profile : INotifyPropertyChanged {
		public int id;
		private string _nickname;
		public string nickname {
			get {
				return _nickname;
			}
			set {
				if (_nickname != value) {
					_nickname = value;
					NotifyPropertyChanged("nickname");
				}
			}
		}
		public Rank rank { get; set; }
		public List<Rate> rates = null;
		public Rate primaryRate { get; set; } = Rate.UNDESIGNATED;
		public Vessel assignedShip = null;
		public Privs privs { get; set; } = null;

		public event PropertyChangedEventHandler PropertyChanged;

		[MessagePackIgnore]
		public List<Rate> wpfRates { get { return rates; } }

		public Profile() {
			id = 0;
			nickname = "";
			rank = null;
			rates = null;
			primaryRate = null;
			assignedShip = null;
		}

		public static Profile FromDatamodel(Datamodel.User u, List<Datamodel.StruckRate> r) {
			Profile p = new Profile();

			p.id = u.id;
			p.nickname = u.name;
			p.rank = Rank.FromDatamodel(u.Rank);
			p.rates = Rate.FromDatamodel(r);
			p.privs = Privs.FromDatamodel(u.Privs);

			// Go through the rate array and find which one matches the primary
			if (r != null) {
				for (int i = 0; i < r.Count; ++i) {
					if (r[i].id == u.rate) {
						p.primaryRate = Rate.FromDatamodel(r[i]);
						break;
					}
				}
			}

			return p;
		}

		protected void NotifyPropertyChanged(string name) {
			if(PropertyChanged != null) {
				PropertyChanged(this, new PropertyChangedEventArgs(name));
			}
		}
	}
}
