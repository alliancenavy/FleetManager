namespace ANWI.Messaging {

	/// <summary>
	/// Server -> Client
	/// Information on a vessel requested by the client
	/// </summary>
	public class FullVessel : IMessagePayload {
		public Vessel vessel;

		public FullVessel() {
			vessel = null;
		}

		public FullVessel(Vessel v) {
			vessel = v;
		}

		public override string ToString() {
			return "Type: FullVessel";
		}
	}
}
