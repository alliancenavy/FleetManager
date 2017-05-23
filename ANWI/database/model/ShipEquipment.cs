using System;
using System.Collections.Generic;
using System.Data.SQLite;

namespace ANWI.Database.Model {

	/// <summary>
	/// Represents a row of the ShipEquipment table
	/// </summary>
	public class ShipEquipment {
		#region Model
		public int id;
		public int hull;
		public int ship;

		public int count { get; private set; }

		private ShipEquipment(int id, int hull, int ship, int count) {
			this.id = id;
			this.hull = hull;
			this.ship = ship;
			this.count = count;
		}
		#endregion

		#region Class-Members
		public static ShipEquipment Factory() {
			ShipEquipment result = new ShipEquipment(
				id: -1,
				hull: -1,
				ship: -1,
				count: 0
			);
			return result;
		}

		public static ShipEquipment Factory(int id, int hull, int ship) {
			ShipEquipment result = new ShipEquipment(
				id: id,
				hull: hull,
				ship: ship,
				count: 1
			);
			return result;
		}

		public static ShipEquipment Factory(SQLiteDataReader reader) {
			ShipEquipment result = new ShipEquipment(
				id: Convert.ToInt32(reader["id"]),
				hull: Convert.ToInt32(reader["hull"]),
				ship: Convert.ToInt32(reader["ship"]),
				count: Convert.ToInt32(reader["count"])
			);
			return result;
		}

		/// <summary>
		/// Creates a new piece of equipment embarked on a ship
		/// </summary>
		/// <param name="output"></param>
		/// <param name="hull"></param>
		/// <param name="ship"></param>
		/// <returns></returns>
		public static bool Create(ref ShipEquipment output, int hull, 
			int ship) {
			int result = DBI.DoPreparedAction(
				@"INSERT INTO ShipEquipment (hull, ship) 
				VALUES (@hull, @ship);",
				new Tuple<string, object>("@hull", hull), 
				new Tuple<string, object>("@ship", ship));
			if(result == 1) {
				return ShipEquipment.FetchById(ref output, DBI.LastInsertRowId);
			}
			return false;
		}

		/// <summary>
		/// Fetches a single row by ID.
		/// </summary>
		/// <param name="output"></param>
		/// <param name="id"></param>
		/// <returns></returns>
		public static bool FetchById(ref ShipEquipment output, int id) {
			SQLiteDataReader reader = DBI.DoPreparedQuery(
				@"SELECT id, hull, ship, 0 AS count
				FROM ShipEquipment WHERE id = @id;",
				new Tuple<string, object>("@id", id));
			if(reader != null && reader.Read()) {
				output = ShipEquipment.Factory(reader);
				return true;
			}
			return false;
		}

		/// <summary>
		/// Fetches the count of a certain hull on a ship
		/// </summary>
		/// <param name="output"></param>
		/// <param name="hullId"></param>
		/// <param name="shipId"></param>
		/// <returns></returns>
		public static bool FetchHullByShip(ref ShipEquipment output, int hullId,
			int shipId) {
			SQLiteDataReader reader = DBI.DoPreparedQuery(
				@"SELECT id, hull, ship, COUNT(id) AS count
				FROM ShipEquipment
				WHERE ship = @ship AND hull = @hull
				GROUP BY hull;",
				new Tuple<string, object>("@ship", shipId), 
				new Tuple<string, object>("@hull", hullId));
			if (reader != null && reader.Read()) {
				output = ShipEquipment.Factory(reader);
				return true;
			}
			return false;
		}

		/// <summary>
		/// Selects all hulls embarked on a given ship
		/// </summary>
		/// <param name="output"></param>
		/// <param name="shipId"></param>
		/// <returns></returns>
		public static bool FetchAllByShip(ref List<ShipEquipment> output,
			int shipId) {
			output = new List<ShipEquipment>();

			SQLiteDataReader reader = DBI.DoPreparedQuery(
				@"SELECT 0 AS id, hull, ship, COUNT(id) AS count
				FROM ShipEquipment
				WHERE ship = @ship
				GROUP BY hull;",
				new Tuple<string, object>("@ship", shipId));
			while(reader != null && reader.Read()) {
				output.Add(ShipEquipment.Factory(reader));
			}

			return true;
		}

		/// <summary>
		/// Updates a row
		/// </summary>
		/// <param name="input"></param>
		/// <returns></returns>
		public static bool Store(ShipEquipment input) {
			int result = DBI.DoPreparedAction(
				@"UPDATE ShipEquipment
				SET hull = @hull, ship = @ship
				WHERE id = @id;",
				new Tuple<string, object>("@hull", input.hull), 
				new Tuple<string, object>("@ship", input.ship), 
				new Tuple<string, object>("@id", input.id));
			if (result == 1)
				return true;
			else
				return false;
		}

		/// <summary>
		/// Deletes a single instance of a hull on a ship
		/// </summary>
		/// <param name="hullId"></param>
		/// <returns></returns>
		public static bool DeleteOneOfHullOnShip(int hullId, int shipId) {
			int result = DBI.DoPreparedAction(
				@"DELETE FROM ShipEquipment
				WHERE id IN (
					SELECT id FROM ShipEquipment
					WHERE hull = @hull AND ship = @ship
				);",
				new Tuple<string, object>("@hull", hullId), 
				new Tuple<string, object>("@ship", shipId));
			if (result == 1)
				return true;
			else
				return false;
		}
		#endregion
	}
}
