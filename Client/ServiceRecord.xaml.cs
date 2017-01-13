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

		public ServiceRecord(Profile p) {
			InitializeComponent();

			//
			// Populate fields
			Text_Nickname.Text = p.rank.abbrev + " " + p.nickname;

			// Rank
			Text_Rank.Text = p.rank.name;
			// TODO: rank image

			// Primary Rate
			Rate pr = p.rates[p.primaryRate];
			Text_PrimaryRate.Text = pr.name + " " + pr.getClass() + " Class";
			// TODO: rate image

			// Time in service
			// TODO

			// Assigned ship
			Text_CurrentShip.Text = p.assignedShip.name + " (" + p.assignedShip.hull.type 
				+ " class " + p.assignedShip.hull.role + ")";

		}
	}
}
