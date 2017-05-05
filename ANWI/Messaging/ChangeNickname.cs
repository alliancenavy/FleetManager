namespace ANWI.Messaging {

	/// <summary>
	/// Client -> Server
	/// Payload for changing a user's name
	/// </summary>
	public class ChangeNickname : IMessagePayload {
		public string auth0_id;
		public string newName;

		public ChangeNickname() {
			auth0_id = "";
			newName = "";
		}

		public ChangeNickname(string id, string nick) {
			auth0_id = id;
			newName = nick;
		}

		public override string ToString() {
			return $"Type: ChangeNickname. Id: {auth0_id}. New Name: {newName}";
		}
	}
}
