namespace ANWI.Messaging {

	/// <summary>
	/// Server -> Client
	/// Information on a user requested by the client
	/// </summary>
	public class FullProfile : IMessagePayload {
		public Profile profile;

		public FullProfile() {
			profile = null;
		}

		public FullProfile(Profile p) {
			profile = p;
		}

		public override string ToString() {
			return $"Type: FullProfile.";
		}
	}
}
