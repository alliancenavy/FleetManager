using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MsgPack.Serialization;
using System.ComponentModel;

namespace ANWI {

	/// <summary>
	/// Wrapper around LiteProfile for users participating in an Op
	/// </summary>
	public class OpParticipant : INotifyPropertyChanged {
		public event PropertyChangedEventHandler PropertyChanged;

		public LiteProfile profile { get; set; }
		public bool isFC { get; set; }

		[MessagePackIgnore]
		private OpPosition _position = null;
		[MessagePackIgnore]
		public OpPosition position {
			get { return _position; }
			set {
				if(_position != value) {
					_position = value;
					NotifyPropertyChanged("position");
					NotifyPropertyChanged("isAssigned");
				}
			}
		}

		[MessagePackIgnore]
		public bool isAssigned { get { return position != null; } }

		public OpParticipant() {
			profile = null;
			isFC = false;
			position = null;
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
}
