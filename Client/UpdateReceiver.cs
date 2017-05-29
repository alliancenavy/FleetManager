using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client {
	public class UpdateReceiver {

		public string outputPath { get; private set; }

		private long expectedSize;
		private MemoryStream stream = new MemoryStream();
		public int progress {
			get { return (int)(((float)stream.Length / (float)expectedSize) * 100); }
		}
		
		public UpdateReceiver(long size) {
			expectedSize = size;

			outputPath = Path.Combine(
				Path.GetTempPath(), 
				Path.GetRandomFileName());
		}

		public bool AddChunk(byte[] data) {
			stream.Write(data, 0, data.Length);

			return stream.Length < expectedSize;
		}

		public bool Write() {
			try {
				FileStream fileout = new FileStream(outputPath, FileMode.Create);
				stream.Position = 0;
				stream.WriteTo(fileout);
				fileout.Close();
				return true;
			} catch(Exception e) {
				return false;
			}
		}
	}
}
