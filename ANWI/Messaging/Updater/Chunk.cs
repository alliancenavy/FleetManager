using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ANWI.Messaging.Updater {
	/// <summary>
	/// Server -> Client
	/// Chunk of data for the updater
	/// </summary>
	public class Chunk : IMessagePayload {
		public byte[] data;

		public Chunk() {
			data = null;
		}

		public override string ToString() {
			return $"Type: Updater.Chunk.";
		}
	}
}
