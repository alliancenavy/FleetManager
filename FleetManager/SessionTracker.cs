using FleetManager.Services;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FleetManager {
	/// <summary>
	/// Keeps track of which users are currently connected to the server
	/// </summary>
	public class SessionTracker {
		private class Session {
			public Main service;
			public string auth0;
		}

		private Logger logger = LogManager.GetLogger("Session Tracker");

		private Dictionary<string, Session> sessions 
			= new Dictionary<string, Session>();

		public SessionTracker() {
			logger.Info("Started");
		}

		/// <summary>
		/// Starts a new session for a user.
		/// Automatically terminates any existing session for this auth0 id
		/// </summary>
		/// <param name="auth0"></param>
		/// <param name="service"></param>
		public void NewSession(string auth0, Main service) {
			Session existing = GetSession(auth0);
			if(existing != null) {
				TerminateSession(existing);
			}

			Session sesh = new Session() {
				auth0 = auth0,
				service = service
			};

			sessions.Add(auth0, sesh);

			logger.Info($"New session created for {auth0}. [{sessions.Count}]");
		}

		/// <summary>
		/// Ends an active session gracefully
		/// </summary>
		/// <param name="aut0"></param>
		public void EndSession(string auth0) {
			Session existing = GetSession(auth0);
			if (existing == null)
				return;

			sessions.Remove(auth0);

			logger.Info($"Session ended for {auth0}. [{sessions.Count}]");
		}

		/// <summary>
		/// Forces a service to disconnect its user
		/// </summary>
		/// <param name="sesh"></param>
		private void TerminateSession(Session sesh) {
			logger.Info($"Terminating existing session for {sesh.auth0}.");

			sesh.service.Terminate();
		}

		private Session GetSession(string auth0) {
			Session sesh;
			if(!sessions.TryGetValue(auth0, out sesh)) {
				return null;
			}
			return sesh;
		}
	}
}
