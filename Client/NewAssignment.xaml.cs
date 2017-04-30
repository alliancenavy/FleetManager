using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using ANWI;
using WebSocketSharp;
using System.ComponentModel;

namespace Client {
	/// <summary>
	/// Interaction logic for NewAssignment.xaml
	/// </summary>
	public partial class NewAssignment : Window, INotifyPropertyChanged {
		private ObservableCollection<AssignmentRole> roleList = null;
		public ObservableCollection<AssignmentRole> wpfRoleList { get { return roleList; } }

		private ObservableCollection<LiteProfile> unassignedPersonnel = null;
		public ObservableCollection<LiteProfile> wpfUnassignedPersonnel { get { return unassignedPersonnel; } }

		private WebSocket socket;

		public event PropertyChangedEventHandler PropertyChanged;

		public event Action<int, int> returnNewAssignment;

		public NewAssignment(WebSocket socket) {
			roleList = new ObservableCollection<AssignmentRole>(CommonData.assignmentRoles);

			this.DataContext = this;
			InitializeComponent();

			this.socket = socket;
			FetchUnassignedRoster();
		}

		public void SetUnassignedPersonnel(List<LiteProfile> roster) {
			unassignedPersonnel = new ObservableCollection<LiteProfile>(roster);
			NotifyPropertyChanged("wpfUnassignedPersonnel");
			this.Dispatcher.Invoke(() => { Spinner_Roster.Visibility = Visibility.Hidden; });
		}

		private void FetchUnassignedRoster() {
			this.Dispatcher.Invoke(() => { Spinner_Roster.Visibility = Visibility.Visible; });

			ANWI.Messaging.Message.Send(
				socket,
				ANWI.Messaging.Message.Routing.FleetReg,
				new ANWI.Messaging.Request(ANWI.Messaging.Request.Type.GetUnassignedRoster));
		}

		private void Button_OK_Click(object sender, RoutedEventArgs e) {
			LiteProfile profile = List_Roster.SelectedItem as LiteProfile;
			AssignmentRole role = Combo_Role.SelectedItem as AssignmentRole;
			if(profile != null && role != null && returnNewAssignment != null) {
				returnNewAssignment(profile.id, role.id);
			}

			this.Close();
		}

		private void Button_Cancel_Click(object sender, RoutedEventArgs e) {
			this.Close();
		}

		private void NotifyPropertyChanged(string name) {
			if (PropertyChanged != null)
				PropertyChanged(this, new PropertyChangedEventArgs(name));
		}
	}
}
