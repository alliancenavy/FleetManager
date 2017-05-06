using ANWI.Database;
using Datamodel = ANWI.Database.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ANWI {

	/// <summary>
	/// Represents a collection of a certain hull embarked on a larger ship
	/// </summary>
	public class ShipEquipment {
		#region Instance Variables
		public int count { get; set; }

		private int _hullId;
		private Hull _hull = null;
		public Hull hull {
			get {
				if(DBI.IsOpen() && _hull == null) {
					_hull = Hull.FetchById(_hullId);
				}
				return _hull;
			}
			set { _hull = value; }
		}

		public int shipId;
		#endregion

		#region Constructors
		public ShipEquipment() {
			count = 0;
			_hullId = 0;
			shipId = 0;
		}

		private ShipEquipment(Datamodel.ShipEquipment e) {
			count = e.count;
			_hullId = e.hull;
			shipId = e.ship;
		}

		/// <summary>
		/// Gets a summary count of the hulls embarked on a ship
		/// </summary>
		/// <param name="hullId"></param>
		/// <param name="shipId"></param>
		/// <returns></returns>
		public static ShipEquipment FetchByHullShip(int hullId, int shipId) {
			Datamodel.ShipEquipment e = null;
			if(Datamodel.ShipEquipment.FetchHullByShip(ref e, hullId, shipId)) {
				return new ShipEquipment(e);
			} else {
				return null;
			}
		}

		/// <summary>
		/// Gets a list of counts of all hulls embarked on a ship
		/// </summary>
		/// <param name="shipId"></param>
		/// <returns></returns>
		public static List<ShipEquipment> FetchAllByShip(int shipId) {
			List<Datamodel.ShipEquipment> equipment = null;
			Datamodel.ShipEquipment.FetchAllByShip(ref equipment, shipId);

			return equipment.ConvertAll<ShipEquipment>((e) => {
				return new ShipEquipment(e); });
		}
		#endregion
	}
}
