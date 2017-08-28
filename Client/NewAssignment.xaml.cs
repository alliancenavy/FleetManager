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
	public partial class NewAssignment : MailboxWindow, INotifyPropertyChanged {
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

		public event Action<int, int> returnNewAssignment;

		public NewAssignment() {
			_roleList = new ObservableCollection<AssignmentRole>(
				CommonData.assignmentRoles);

			this.DataContext = this;
			InitializeComponent();

			this.AddProcessor(typeof(ANWI.Messaging.FullRoster), ProcessRoster);

			FetchUnassignedRoster();
		}

		/// <summary>
		/// Sends a request for the roster of unassigned personnel
		/// </summary>
		private void FetchUnassignedRoster() {
			this.Dispatcher.Invoke(() => {
				Spinner_Roster.Visibility = Visibility.Visible;
			});

			MessageRouter.Instance.Send(
				MessageRouter.Service.Main,
				new ANWI.Messaging.Request(
					ANWI.Messaging.Request.Type.GetUnassignedRoster),
				this
				);
		}
		
		/// <summary>
		/// Fills the list of available personnel
		/// </summary>
		/// <param name="p"></param>
		public void ProcessRoster(ANWI.Messaging.IMessagePayload p) {
			ANWI.Messaging.FullRoster roster = p as ANWI.Messaging.FullRoster;

			_unassignedPersonnel 
				= new ObservableCollection<LiteProfile>(roster.members);

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
				this.Close();
			}
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
