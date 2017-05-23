using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ANWI.Database.Model {

	/// <summary>
	/// Non-permanent assignments in an operation
	/// </summary>
	public class OperationRole {
		#region Model
		public int id;
		public string name;
		public int rate;
		public bool onShips;
		public bool onBoats;
		public bool inSquads;
		public bool channelCdr;

		private OperationRole(int id, string name, int rate, bool ships,
			bool boats, bool squads, bool channelCdr) {
			this.id = id;
			this.name = name;
			this.rate = rate;
			this.onShips = ships;
			this.onBoats = boats;
			this.inSquads = squads;
			this.channelCdr = channelCdr;
		}
		#endregion

		#region Class Members
		public static OperationRole Factory() {
			OperationRole result = new OperationRole(
				id: -1,
				name: "",
				rate: -1,
				ships: false,
				boats: false,
				squads: false,
				channelCdr: false
				);
			return result;
		}

		public static OperationRole Factory(int id, string name, int rate,
			bool ships, bool boats, bool squads, bool chanCdr) {
			OperationRole result = new OperationRole(
				id: id,
				name: name,
				rate: rate,
				ships: ships,
				boats: boats,
				squads: squads,
				channelCdr: chanCdr
				);
			return result;
		}

		public static OperationRole Factory(SQLiteDataReader reader) {
			OperationRole result = new OperationRole(
				id: Convert.ToInt32(reader["id"]),
				name: (string)reader["name"],
				rate: Convert.ToInt32(reader["associatedRate"]),
				ships: Convert.ToBoolean(reader["onShips"]),
				boats: Convert.ToBoolean(reader["onBoats"]),
				squads: Convert.ToBoolean(reader["inSquads"]),
				channelCdr: Convert.ToBoolean(reader["channelCdr"])
				);
			return result;
		}
		
		public static bool Create(ref OperationRole output, string name,
			int rate, bool ships, bool boats, bool squads, bool chanCdr) {
			int result = DBI.DoPreparedAction(
				@"INSERT INTO OperationRole 
				(name, rate, onShips, onBoats, inSquads, channelCdr)
				VALUES (@name, @rate, @ships, @boats, @squads, @chanCdr);",
				new Tuple<string, object>("@name", name), 
				new Tuple<string, object>("@rate", rate), 
				new Tuple<string, object>("@ships", ships), 
				new Tuple<string, object>("@boats", boats), 
				new Tuple<string, object>("@squads", squads),
				new Tuple<string, object>("@chanCdr", chanCdr));
			if(result == 1) {
				return OperationRole.FetchById(ref output, DBI.LastInsertRowId);
			}
			return false;
		}

		/// <summary>
		/// Fetches a specific operation role
		/// </summary>
		/// <param name="output"></param>
		/// <param name="id"></param>
		/// <returns></returns>
		public static bool FetchById(ref OperationRole output, int id) {
			SQLiteDataReader reader = DBI.DoPreparedQuery(
				"SELECT * FROM OperationRole WHERE id = @id LIMIT 1;",
				new Tuple<string, object>("@id", id));
			if(reader != null && reader.Read()) {
				output = OperationRole.Factory(reader);
				return true;
			}
			return false;
		}

		/// <summary>
		/// Fetches all of the operation roles which can be assigned to a ship
		/// </summary>
		/// <param name="output"></param>
		/// <returns></returns>
		public static bool FetchAllShips(ref List<OperationRole> output) {
			output = new List<OperationRole>();

			SQLiteDataReader reader = DBI.DoQuery(
				@"SELECT * FROM OperationRole
				WHERE onShips = 1
				ORDER BY id ASC;");
			while(reader != null && reader.Read()) {
				output.Add(OperationRole.Factory(reader));
			}

			return true;
		}

		/// <summary>
		/// Fetches all operation roles which can be assigned to a boat
		/// </summary>
		/// <param name="output"></param>
		/// <returns></returns>
		public static bool FetchAllBoats(ref List<OperationRole> output) {
			output = new List<OperationRole>();

			SQLiteDataReader reader = DBI.DoQuery(
				@"SELECT * FROM OperationRole
				WHERE onBoats = 1
				ORDER BY id ASC;");
			while (reader != null && reader.Read()) {
				output.Add(OperationRole.Factory(reader));
			}

			return true;
		}
		#endregion
	}
}
