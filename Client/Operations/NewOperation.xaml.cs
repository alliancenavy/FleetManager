using System;
using System.Windows;
using ANWI;

namespace Client.Operations {
	/// <summary>
	/// Interaction logic for NewOperation.xaml
	/// </summary>
	public partial class NewOperation : Window {

		public event Action<string, OperationType> returnNewOp;

		public NewOperation() {
			InitializeComponent();
		}

		private void Button_OK_Click(object sender, RoutedEventArgs e) {
			if(returnNewOp != null) {
				if (Text_OpName.Text != "" && Combobox_Type.SelectedIndex != -1) {
					returnNewOp(Text_OpName.Text,
						(OperationType)Combobox_Type.SelectedIndex);
					this.Close();
				}
			}
		}

		private void Button_Cancel_Click(object sender, RoutedEventArgs e) {
			this.Close();
		}
	}
}
