using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ANWI.Messaging {
	/// <summary>
	/// Server -> Client
	/// Chunk of data for the updater
	/// </summary>
	public class UpdateChunk : IMessagePayload {
		public byte[] data;

		public UpdateChunk() {
			data = null;
		}

		public override string ToString() {
			return $"Type: UpdateChunk.";
		}
	}
}
