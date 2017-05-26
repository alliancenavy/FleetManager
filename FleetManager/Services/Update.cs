using ANWI.Messaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FleetManager.Services {
	public class Update : BaseService {

		#region Instance Members
		#endregion

		#region Constructors
		public Update() : base("Update Service") {
			AddProcessor(typeof(ANWI.Messaging.CheckUpdate),
				ProcessCheckUpdate);
			AddProcessor(typeof(ANWI.Messaging.Request),
				ProcessRequest);
		}
		#endregion

		#region Cookie Stubs
		protected override string GetLogIdentifier() {
			return "";
		}
		#endregion

		#region Message Processors
		private IMessagePayload ProcessCheckUpdate(IMessagePayload p) {
			CheckUpdate check = p as CheckUpdate;

			if (check.ver < Configuration.clientVersion) {
				return new UpdateStatus() {
					ver = Configuration.clientVersion,
					updateNeeded = true,
					updateSize = 0
				};
			} else {
				return new UpdateStatus() {
					ver = check.ver,
					updateNeeded = false,
					updateSize = 0
				};
			}
		}

		private IMessagePayload ProcessRequest(IMessagePayload p) {
			ANWI.Messaging.Request req = p as ANWI.Messaging.Request;

			if (req.type != Request.Type.GetUpdateChunk)
				return null;

			return new ANWI.Messaging.UpdateChunk() {
				data = new byte[100]
			};
		}
		#endregion
	}
}
