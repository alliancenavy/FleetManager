using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace ANWI.Utility {
	public static class MD5List {

		public static Dictionary<string, string> 
		GetDirectoryChecksum(string directory) {
			if(Directory.Exists(directory)) {
				Dictionary<string, string> output 
					= new Dictionary<string, string>();

				string[] files = Directory.GetFiles(directory);
				foreach(string file in files) {
					using(MD5 md5 = MD5.Create()) {
						FileStream stream = File.OpenRead(file);
						output.Add(
							Path.GetFileName(file),
							Encoding.Default.GetString(md5.ComputeHash(stream))
							);
					}
				}

				return output;
			} else {
				return null;
			}
		}

	}
}
