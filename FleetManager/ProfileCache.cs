using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NLog;
using ANWI;

namespace FleetManager {
	/// <summary>
	/// Stores user profiles in a shared manner so that when one changes
	/// all services get the update.
	/// </summary>
	public class ProfileCache {

		private class CachedProfile {
			private long time;
			public LiteProfile p { get; private set; }

			public CachedProfile(LiteProfile p) {
				this.p = p;
				time = DateTime.UtcNow.Ticks;
			}

			public bool Expired { get {
					return DateTime.UtcNow.Ticks - time > TimeSpan.TicksPerMinute * 10;
				} }

			public void Refresh() {
				time = DateTime.UtcNow.Ticks;
				p.Refresh();
			}
		}

		#region Instance Members
		private Logger logger = LogManager.GetLogger("Profile Cache");

		private Dictionary<string, CachedProfile> profiles
			= new Dictionary<string, CachedProfile>();
		#endregion

		#region Constructors
		private static ProfileCache _instance = null;
		public static ProfileCache Instance {
			get {
				if (_instance == null)
					_instance = new ProfileCache();
				return _instance;
			}
		}

		private ProfileCache() {
			// Empty
		}
		#endregion

		#region Interface
		public LiteProfile GetProfile(string auth0) {
			CachedProfile cp = null;
			if(!profiles.TryGetValue(auth0, out cp)) {
				LiteProfile p = LiteProfile.FetchByAuth0(auth0);
				if (p == null)
					return null;

				cp = new CachedProfile(p);
				profiles.Add(auth0, cp);
			}

			if (cp.Expired)
				cp.Refresh();

			return cp.p;
		}

		public LiteProfile GetProfile(int id) {
			foreach(KeyValuePair<string, CachedProfile> pair in profiles) {
				if (pair.Value.p.id == id)
					return pair.Value.p;
			}

			LiteProfile p = LiteProfile.FetchById(id);
			if (p == null)
				return null;

			CachedProfile cp = new CachedProfile(p);
			profiles.Add(p.auth0, cp);

			return p;
		}
		#endregion

	}
}
