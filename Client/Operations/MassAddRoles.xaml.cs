using ANWI;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using Xceed.Wpf.Toolkit;

namespace Client.Operations {
	/// <summary>
	/// Interaction logic for MassAddRoles.xaml
	/// </summary>
	public partial class MassAddRoles : Window {

		public class RoleCount : INotifyPropertyChanged {
			public event PropertyChangedEventHandler PropertyChanged;

			public OperationRole role { get; set; }

			private int _count = 0;
			public int count {
				get { return _count; }
				set {
					if(_count != value) {
						_count = value;
						NotifyPropertyChanged("count");
					}
				}
			}

			/// <summary>
			/// Notifies the UI when a bound property changes
			/// </summary>
			/// <param name="name"></param>
			public void NotifyPropertyChanged(string name) {
				if (PropertyChanged != null) {
					PropertyChanged(this, new PropertyChangedEventArgs(name));
				}
			}
		}

		public event Action<List<int>> returnNewPositions;

		public List<RoleCount> roles { get; set; }

		public MassAddRoles(List<OperationRole> r) {
			roles = new List<RoleCount>(r.ConvertAll<RoleCount>((a) => {
				return new RoleCount() { role = a };
			}));

			this.DataContext = this;
			InitializeComponent();
		}

		private void Button_OK_Click(object sender, RoutedEventArgs e) {
			List<int> output = new List<int>();

			foreach(RoleCount pos in roles) {
				for(int i = 0; i < pos.count; ++i) {
					output.Add(pos.role.id);
				}
			}

			if (returnNewPositions != null)
				returnNewPositions(output);

			this.Close();
		}

		private void Button_Cancel_Click(object sender, RoutedEventArgs e) {
			this.Close();
		}

		private void Spinner_Count_Spin(object sender, Xceed.Wpf.Toolkit.SpinEventArgs e) {
			RoleCount role = (sender as Spinner).DataContext as RoleCount;
			if(e.Direction == SpinDirection.Increase) {
				if (role.count < 10)
					role.count++;
			} else {
				if (role.count > 0)
					role.count--;
			}
		}
	}
}
