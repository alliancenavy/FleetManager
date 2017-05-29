using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Updater {
	class Program {
		static void Main(string[] args) {

			// Validate command line
			Console.WriteLine("Patcher started");
			if(args.Length < 2) {
				Console.WriteLine("Need at least two arguments");
				Thread.Sleep(1000);
				return;
			}

			// Wait for the launching PID to die
			string parentName = args[0];
			Console.WriteLine($"Waiting for parent {parentName} to close...");
			while(Process.GetProcessesByName(parentName).Length > 0) {
				Thread.Sleep(100);
			}
			Thread.Sleep(2000);

			// Wait for the archive to be available
			string archiveName = args[1];
			while (IsFileInUse(archiveName)) {
				Console.WriteLine("file in use");
				Thread.Sleep(100);
			}
			
			Console.WriteLine("Patching from archive " + archiveName);

			FileStream stream = null;
			try {
				stream = new FileStream(archiveName, FileMode.Open);
				ZipArchive archive = new ZipArchive(stream, ZipArchiveMode.Read);
				
				foreach(ZipArchiveEntry entry in archive.Entries) {
					string fullPath = Path.Combine(".", entry.FullName);
					string directory = Path.GetDirectoryName(fullPath);

					if (!Directory.Exists(directory))
						Directory.CreateDirectory(directory);

					if (entry.Name != "") {
						try {
							entry.ExtractToFile(fullPath, true);
						} catch(Exception e) {

						}
					}
				}
			} catch (Exception e) {
				Console.WriteLine("Failed to patch: " + e);
				Thread.Sleep(2000);
			}

			if (stream != null)
				stream.Close();

			File.Delete(archiveName);

			Process client = new Process();
			client.StartInfo = new ProcessStartInfo("AFOSClient.exe");

			try {
				client.Start();
			} catch(Exception e) {
				Console.WriteLine("Failed to launch client.  Please start it manually.");
				Thread.Sleep(2000);
			}
		}

		private static bool IsFileInUse(string name) {
			Console.WriteLine("Checking file " + name);
			bool locked = false;
			try {
				FileStream fs =
					File.Open(name, FileMode.Open,
					FileAccess.ReadWrite, FileShare.None);
				fs.Close();
			} catch (IOException ex) {
				locked = true;
			}
			return locked;
		}
	}
}
