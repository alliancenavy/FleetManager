using System;
using System.Collections.Generic;
using System.ComponentModel;
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

namespace Client {
	/// <summary>
	/// Interaction logic for Splash.xaml
	/// </summary>
	public partial class Splash : MailboxWindow, INotifyPropertyChanged {

		public event PropertyChangedEventHandler PropertyChanged;

		public int progress { get; private set; } = 0;

		private bool _updating = false;
		public bool updating {
			get { return _updating; }
			set {
				if(_updating != value) {
					_updating = value;
					NotifyPropertyChanged("updating");
				}
			}
		}

		public Splash() {
			AddProcessor(typeof(ANWI.Messaging.UpdateStatus),
				ProcessUpdateStatus);
			AddProcessor(typeof(ANWI.Messaging.UpdateChunk),
				ProcessUpdateChunk);

			this.DataContext = this;
			InitializeComponent();

			MessageRouter.Instance.ConnectUpdate(this);
			MessageRouter.Instance.SendUpdate(new ANWI.Messaging.CheckUpdate() {
				ver = CommonData.version
			});
		}

		private void ProcessUpdateStatus(ANWI.Messaging.IMessagePayload p) {
			ANWI.Messaging.UpdateStatus up
				= p as ANWI.Messaging.UpdateStatus;

			if(up.updateNeeded) {
				this.Dispatcher.Invoke(() => {
					Text_Message.Text = "Downloading Update.";
				});

				Thread.Sleep(1000);
				updating = true;
				Thread.Sleep(1000);

				MessageRouter.Instance.SendUpdate(new ANWI.Messaging.Request() {
					type = ANWI.Messaging.Request.Type.GetUpdateChunk
				});
			} else {
				this.Dispatcher.Invoke(() => {
					Text_Message.Text = "Version OK.";
				});

				Thread.Sleep(1000);

				this.Dispatcher.Invoke(() => {
					this.Close();
				});
			}
		}

		private void ProcessUpdateChunk(ANWI.Messaging.IMessagePayload p) {
			ANWI.Messaging.UpdateChunk chunk
				= p as ANWI.Messaging.UpdateChunk;

			progress += 5;
			NotifyPropertyChanged("progress");

			Thread.Sleep(1000);
			MessageRouter.Instance.SendUpdate(new ANWI.Messaging.Request() {
				type = ANWI.Messaging.Request.Type.GetUpdateChunk
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
	}
}
