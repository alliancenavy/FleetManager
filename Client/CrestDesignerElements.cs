using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client {
	namespace CrestDesignerElements {

		public class Element : INotifyPropertyChanged {
			public event PropertyChangedEventHandler PropertyChanged;

			private int _size = 100;
			public int Size {
				get { return _size; }
				set {
					if(_size != value) {
						_size = value;
						NotifyPropertyChanged("Size");
					}
				}
			}

			private int _x = 0;
			public int X {
				get { return _x; }
				set {
					if (_x != value) {
						_x = value;
						NotifyPropertyChanged("X");
						NotifyPropertyChanged("Position");
					}
				}
			}

			private int _y = 0;
			public int Y {
				get { return _y; }
				set {
					if(_y != value) {
						_y = value;
						NotifyPropertyChanged("Y");
						NotifyPropertyChanged("Position");
					}
				}
			}

			public string Position { get {
					return $"{X},{Y},0,0";
				} }

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

		public class Silhouetted : Element {
			public string Image { get; set; }
			public string Name { get; set; }

			private string _color = "Black";
			public string Color {
				get { return _color; }
				set {
					if(_color != value) {
						_color = value;
						NotifyPropertyChanged("Color");
					}
				}
			}
		}

	}
}
