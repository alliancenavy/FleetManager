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
			Text_CurrentShip.Text = p.assignedShip.name + " (" + p.assignedShip.hull.type 
				+ " class " + p.assignedShip.hull.role + ")";

			// Awards list
			List_Awards.Items.Add(new AwardRecord() {
				Name = "Order of the Cool Guy (x1)",
				Icon = "images/no_image.png"
			});

			List_Awards.Items.Add(new AwardRecord() {
				Name = "Good Conduct Medal (x2)",
				Icon = "images/no_image.png"
			});

			List_Awards.Items.Add(new AwardRecord() {
				Name = "Command Star (x1)",
				Icon = "images/no_image.png"
			});
		}
	}
}
