using System;

namespace ANWI.Utility {
	public static class UUID {
		/// <summary>
		/// Generates a UUID for an Op.
		/// From: http://madskristensen.net/post/generate-unique-strings-and-numbers-in-c
		/// </summary>
		/// <returns></returns>
		public static string GenerateUUID() {
			long i = 1;
			foreach (byte b in Guid.NewGuid().ToByteArray()) {
				i *= ((int)b + 1);
			}
			return string.Format("{0:x}", i - DateTime.Now.Ticks);
		}
	}
}
