namespace ANWI.Messaging.ReqExp {

	/// <summary>
	/// Request expounding detail
	/// An ID number and a string
	/// </summary>
	public class IdString : IRequestDetail {
		public int id;
		public string str;

		public IdString() {
			id = 0;
			str = "";
		}

		public IdString(int i, string s) {
			id = i;
			str = s;
		}
	}
}
