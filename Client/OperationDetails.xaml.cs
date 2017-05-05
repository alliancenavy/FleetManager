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
using System.Collections.ObjectModel;
using ANWI;

namespace Client {
	/// <summary>
	/// Interaction logic for OperationDetails.xaml
	/// </summary>
	public partial class OperationDetails : Window {

		//private ObservableCollection<RosterEntry> roster = new ObservableCollection<RosterEntry>();

		//public ObservableCollection<RosterEntry> wpfRoster { get { return roster; } }

		public OperationDetails() {
			this.DataContext = this;
			InitializeComponent();

			/*RosterEntry re = new RosterEntry();
			re.name = "Mazer Ludd";
			re.rank = new Rank();
			re.rank.abbrev = "CAPT";
			re.rank.ordering = 5;
			re.primaryRate = new Rate();
			re.primaryRate.abbrev = "SK";
			re.primaryRate.rank = 2;
			roster.Add(re);*/
		}
	}
}
