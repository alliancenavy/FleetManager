using MsgPack.Serialization;
using System.ComponentModel;

namespace ANWI {

	/// <summary>
	/// A position on a ship in an operation
	/// </summary>
	public class OpPosition : INotifyPropertyChanged {

		public event PropertyChangedEventHandler PropertyChanged;

		public string uuid;
		public string unitUUID;
		public OperationRole role { get; set; }
		public bool critical { get; set; }

		[MessagePackIgnore]
		private int _filledById;
		public int filledById {
			get { return _filledById; }
			set {
				if (_filledById != value) {
					_filledById = value;
					NotifyPropertyChanged("filledById");
				}
			}
		}

		[MessagePackIgnore]
		private OpParticipant _filledByPointer;
		[MessagePackIgnore]
		public OpParticipant filledByPointer {
			get { return _filledByPointer; }
			set {
				if (_filledByPointer != value) {
					_filledByPointer = value;
					NotifyPropertyChanged("filledByPointer");
					NotifyPropertyChanged("isFilled");
					NotifyPropertyChanged("criticalAndUnfilled");
				}
			}
		}

		public bool isFilled { get { return _filledByPointer != null; } }
		public bool criticalAndUnfilled { get {
				return !isFilled && critical;
			} }

		public OpPosition() {
			filledById = -1;
			filledByPointer = null;
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
