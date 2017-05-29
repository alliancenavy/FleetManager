using ANWI.Messaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ANWI.Messaging.Updater;
using ANWI.Utility;
using System.IO.Compression;
using System.IO;

namespace FleetManager.Services {
	public class Update : BaseService {

		#region Instance Members
		private MemoryStream zipStream = null;
		private byte[] buffer = new byte[8192];
		#endregion

		#region Constructors
		public Update() : base("Update Service") {
			AddProcessor(typeof(ANWI.Messaging.Updater.Check),
				ProcessCheckUpdate);
			AddProcessor(typeof(ANWI.Messaging.Request),
				ProcessRequest);
		}
		#endregion

		#region Cookie Stubs
		protected override string GetLogIdentifier() {
			return Context.CookieCollection["name"].Value;
		}
		#endregion

		#region Message Processors
		private IMessagePayload ProcessCheckUpdate(IMessagePayload p) {
			Check check = p as Check;

			if(Configuration.updateDir == null) {
				return new CheckResult() {
					updateNeeded = false
				};
			}

			Dictionary<string, string> localChecksums 
				= MD5List.GetDirectoryChecksum(Configuration.updateDir);

			List<string> differences 
				= GetFileDifferences(localChecksums, check.checksums);

			if(differences.Count == 0) {
				logger.Info($"Client {GetLogIdentifier()} is up to date");

				return new CheckResult() {
					updateNeeded = false
				};
			} else {
				string differenceString = "";
				foreach(string file in differences) {
					differenceString += $"{file} ";
				}

				logger.Info(
					$"Client {GetLogIdentifier()} has the following files " +
					"out of date: " + differenceString);

				// Package up the files that don't match for the client 
				// to request in chunks
				zipStream = new MemoryStream();
				using (ZipArchive archive 
					= new ZipArchive(zipStream, ZipArchiveMode.Create, true)) {

					foreach (string file in differences) {
						archive.CreateEntryFromFile(
							Path.Combine(Configuration.updateDir, file),
							file);
					}

					archive.Dispose();
				}

				zipStream.Position = 0;

				return new CheckResult() {
					updateNeeded = true,
					updateSize = zipStream.Length
				};
			}
		}

		private IMessagePayload ProcessRequest(IMessagePayload p) {
			ANWI.Messaging.Request req = p as ANWI.Messaging.Request;

			if (req.type != Request.Type.GetUpdateChunk)
				return null;

			zipStream.Read(buffer, 0, buffer.Length);
			return new ANWI.Messaging.Updater.Chunk() {
				data = buffer
			};
		}
		#endregion

		#region Helpers
		private List<string> 
		GetFileDifferences(Dictionary<string, string> local, 
		Dictionary<string, string> client) {
			List<string> output = new List<string>();

			foreach(KeyValuePair<string, string> pair in local) {
				string checksum;
				if(client.TryGetValue(pair.Key, out checksum)) {
					if (pair.Value != checksum)
						output.Add(pair.Key);
				} else {
					output.Add(pair.Key);
				}
			}

			return output;
		}
		#endregion
	}
}
