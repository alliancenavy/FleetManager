using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client {

	/// <summary>
	/// For managing application data folder
	/// </summary>
	public static class Appdata {

#if DEBUG
		private static string appdataFolder = "appdata";
#else
		private static string appdataFolder
			= Path.Combine(
				Environment.GetFolderPath(
					Environment.SpecialFolder.ApplicationData
					),
				"AFOS Client"
				);
#endif

		/// <summary>
		/// Checks if a file exists in the appdata directory
		/// </summary>
		/// <param name="filename"></param>
		/// <returns></returns>
		public static bool CheckFileExists(string filename) {
			VerifyAppDataDir();
			return File.Exists(GetFullPath(filename));
		}

		/// <summary>
		/// Deletes a file from the appdata directory
		/// </summary>
		/// <param name="filename"></param>
		public static void DeleteFile(string filename) {
			VerifyAppDataDir();
			File.Delete(GetFullPath(filename));
		}

		/// <summary>
		/// Returns a StreamReader for a plaintext file
		/// </summary>
		/// <param name="filename"></param>
		/// <returns></returns>
		public static StreamReader OpenTextFile(string filename) {
			VerifyAppDataDir();
			return File.OpenText(GetFullPath(filename));
		}

		/// <summary>
		/// Returns a stream reader for an encrypted file
		/// </summary>
		/// <param name="filename"></param>
		/// <returns></returns>
		public static StreamReader OpenTextFileEncrypted(string filename) {
			VerifyAppDataDir();
			string path = GetFullPath(filename);
			File.Decrypt(path);
			return File.OpenText(path);
		}

		/// <summary>
		/// Writes a file to the appdata directory
		/// </summary>
		/// <param name="filename"></param>
		/// <param name="text"></param>
		public static void WriteFile(string filename, string text) {
			VerifyAppDataDir();
			File.WriteAllText(GetFullPath(filename), text);
		}

		/// <summary>
		/// Writes a file to the appdata directory and then encrypts it
		/// </summary>
		/// <param name="filename"></param>
		/// <param name="text"></param>
		public static void WriteFileEncrypted(string filename, string text) {
			VerifyAppDataDir();
			string path = GetFullPath(filename);
			File.WriteAllText(path, text);
			File.Encrypt(path);
		}

		/// <summary>
		/// Verifies that the appdata directory exists.
		/// Creates if it does not.
		/// </summary>
		private static void VerifyAppDataDir() {
			if (!Directory.Exists(appdataFolder))
				Directory.CreateDirectory(appdataFolder);
		}

		/// <summary>
		/// Concats the appdata directory with the filename
		/// </summary>
		/// <param name="filename"></param>
		/// <returns></returns>
		private static string GetFullPath(string filename) {
			return Path.Combine(appdataFolder, filename);
		}

	}
}
