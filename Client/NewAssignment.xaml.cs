using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using ANWI;
using WebSocketSharp;
using System.ComponentModel;

namespace Client {
	/// <summary>
	/// Window for assigning a user to a ship
	/// </summary>
	public partial class NewAssignment : Window, INotifyPropertyChanged {
		public event PropertyChangedEventHandler PropertyChanged;

		// List of all roles
		private ObservableCollection<AssignmentRole> _roleList = null;
		public ObservableCollection<AssignmentRole> roleList {
			get { return _roleList; }
		}

		// List of unassigned personnel who can be assigned to this ship
		private ObservableCollection<LiteProfile> _unassignedPersonnel = null;
		public ObservableCollection<LiteProfile> unassignedPersonnel {
			get { return _unassignedPersonnel; }
		}

		private WebSocket socket;

		public event Action<int, int> returnNewAssignment;

		public NewAssignment(WebSocket socket) {
			_roleList = new ObservableCollection<AssignmentRole>(
				CommonData.assignmentRoles);

			this.DataContext = this;
			InitializeComponent();

			this.socket = socket;
			FetchUnassignedRoster();
		}

		/// <summary>
		/// Sends a request for the roster of unassigned personnel
		/// </summary>
		private void FetchUnassignedRoster() {
			this.Dispatcher.Invoke(() => {
				Spinner_Roster.Visibility = Visibility.Visible;
			});

			ANWI.Messaging.Message.Send(
				socket,
				ANWI.Messaging.Message.Routing.FleetReg,
				new ANWI.Messaging.Request(
					ANWI.Messaging.Request.Type.GetUnassignedRoster));
		}

		/// <summary>
		/// When the Fleet window receives the roster response it passes the
		/// list to this window using this function.
		/// </summary>
		/// <param name="roster"></param>
		public void SetUnassignedPersonnel(List<LiteProfile> roster) {
			_unassignedPersonnel 
				= new ObservableCollection<LiteProfile>(roster);

			NotifyPropertyChanged("unassignedPersonnel");

			this.Dispatcher.Invoke(() => {
				Spinner_Roster.Visibility = Visibility.Hidden;
			});
		}
		
		/// <summary>
		/// Submits the newly selected user and their role to the fleet window
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void Button_OK_Click(object sender, RoutedEventArgs e) {
			LiteProfile profile = List_Roster.SelectedItem as LiteProfile;
			AssignmentRole role = Combo_Role.SelectedItem as AssignmentRole;
			if(profile != null && role != null && returnNewAssignment != null) {
				returnNewAssignment(profile.id, role.id);
			}

			this.Close();
		}

		/// <summary>
		/// Closes the window with no changes
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void Button_Cancel_Click(object sender, RoutedEventArgs e) {
			this.Close();
		}

		/// <summary>
		/// Notifies the UI that a bound property has changed
		/// </summary>
		/// <param name="name"></param>
		private void NotifyPropertyChanged(string name) {
			if (PropertyChanged != null)
				PropertyChanged(this, new PropertyChangedEventArgs(name));
		}
	}
}
