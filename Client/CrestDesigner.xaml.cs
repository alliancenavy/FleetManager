using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Client.CrestDesignerElements;
using Xceed.Wpf.Toolkit;

namespace Client {
	/// <summary>
	/// Interaction logic for CrestDesigner.xaml
	/// </summary>
	public partial class CrestDesigner : Window, INotifyPropertyChanged {

		public event PropertyChangedEventHandler PropertyChanged;

		private List<Element> _layers = new List<Element>();
		public List<Element> Layers {
			get { return _layers; }
		}

		public CrestDesigner() {
			_layers.Add(new Silhouetted() {
				Image = "/images/rates/FP0.png",
				Name = "FP",
				Color = "Blue",
				Size = 100
			});

			_layers.Add(new Silhouetted() {
				Image = "/images/rates/QM0.png",
				Name = "QM",
				Color = "Red",
				Size = 100,
				X = 50
			});

			this.DataContext = this;
			InitializeComponent();

			/*Task t = new Task(() => {
				Thread.Sleep(1000);
				SaveToFile("crest.png");
			});
			t.Start();*/
		}

		private void SaveToFile(string filename) {
			this.Dispatcher.Invoke(() => {
				RenderTargetBitmap target = new RenderTargetBitmap(
				(int)Items_CrestImage.ActualWidth,
				(int)Items_CrestImage.ActualHeight,
				100d,
				100d,
				PixelFormats.Default
				);

				target.Render(Items_CrestImage);
				BitmapEncoder png = new PngBitmapEncoder();
				png.Frames.Add(BitmapFrame.Create(target));

				using (FileStream stream = File.Open(filename, FileMode.Create, FileAccess.Write)) {
					png.Save(stream);
				}
			});
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

		private void Spinner_X_Spin(object sender, Xceed.Wpf.Toolkit.SpinEventArgs e) {
			Element elem = (sender as Spinner).DataContext as Element;
			if(e.Direction == SpinDirection.Increase) {
				elem.X++;
			} else {
				elem.X--;
			}
		}

		private void Spinner_Y_Spin(object sender, Xceed.Wpf.Toolkit.SpinEventArgs e) {
			Element elem = (sender as Spinner).DataContext as Element;
			if (e.Direction == SpinDirection.Increase) {
				elem.Y++;
			} else {
				elem.Y--;
			}
		}

		private void Spinner_Size_Spin(object sender, Xceed.Wpf.Toolkit.SpinEventArgs e) {
			Element elem = (sender as Spinner).DataContext as Element;
			if (e.Direction == SpinDirection.Increase) {
				elem.Size++;
			} else {
				elem.Size--;
			}
		}

		private void ColorPick_Silh_SelectedColorChanged(object sender, RoutedPropertyChangedEventArgs<Color?> e) {
			if(e.NewValue.HasValue) {
				Silhouetted elem 
					= (sender as ColorPicker).DataContext as Silhouetted;

				string col = "#" + 
					e.NewValue.Value.R.ToString("X2") +
					e.NewValue.Value.G.ToString("X2") +
					e.NewValue.Value.B.ToString("X2");
				elem.Color = col;
			}
		}
	}
}
