using System;
using System.Collections.Generic;
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
using ANWI.Messaging;

namespace Client {
	/// <summary>
	/// Interaction logic for ServiceRecord.xaml
	/// </summary>
	public partial class ServiceRecord : Window {

		public class AwardRecord {
			public string Name { get; set; }
			public string Icon { get; set; }
		}

		public ServiceRecord(Profile p) {
			InitializeComponent();
			this.DataContext = p;

			// Assigned ship
			if (p.assignedShip == null) {
				Text_CurrentAssignment.Text = "No Current Assignment";
			} else {
				Text_CurrentAssignment.Text = p.assignedShip.name + " (" + p.assignedShip.hull.type
					+ " class " + p.assignedShip.hull.role + ")";
			}
		}

		public void DeliverMessage(Message m) {
			
		}
	}
}
