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
using ANWI;
using ANWI.Utility;
using ANWI.Messaging;
using System.Diagnostics;

namespace Client {
	/// <summary>
	/// Interaction logic for Splash.xaml
	/// </summary>
	public partial class Splash : MailboxWindow, INotifyPropertyChanged {

		public event PropertyChangedEventHandler PropertyChanged;

		private UpdateReceiver receiver = null;

		public Func<string, bool> UpdateDownloaded;

		public int progress {
			get {
				if (receiver == null)
					return 0;
				else
					return receiver.progress;
			}
		}

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
			AddProcessor(typeof(ANWI.Messaging.Updater.CheckResult),
				ProcessUpdateStatus);
			AddProcessor(typeof(ANWI.Messaging.Updater.Chunk),
				ProcessUpdateChunk);

			this.DataContext = this;
			InitializeComponent();

			Task t = new Task(() => {
				try {
					MessageRouter.Instance.ConnectUpdate(this);
					MessageRouter.Instance.SendUpdate(new ANWI.Messaging.Updater.Check() {
						checksums = MD5List.GetDirectoryChecksum(".")
					});
				} catch (Exception e) {
					this.Dispatcher.Invoke(() => {
						Text_Message.Text = "Failed to Connect to Server.";
						Text_Message.Foreground = Brushes.Red;
					});
					Thread.Sleep(2000);
					Process.GetCurrentProcess().Kill();
				}
			});
			t.Start();
		}

		private void ProcessUpdateStatus(ANWI.Messaging.IMessagePayload p) {
			ANWI.Messaging.Updater.CheckResult res
				= p as ANWI.Messaging.Updater.CheckResult;

			if(res.updateNeeded) {
				this.Dispatcher.Invoke(() => {
					Text_Message.Text = "Downloading Update.";
				});

				Thread.Sleep(1000);

				receiver = new UpdateReceiver(res.updateSize);

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
			ANWI.Messaging.Updater.Chunk chunk
				= p as ANWI.Messaging.Updater.Chunk;

			updating = true;
			if (receiver.AddChunk(chunk.data)) {
				NotifyPropertyChanged("progress");
				MessageRouter.Instance.SendUpdate(new ANWI.Messaging.Request() {
					type = ANWI.Messaging.Request.Type.GetUpdateChunk
				});
			} else {
				NotifyPropertyChanged("progress");

				if (!receiver.Write() || 
					UpdateDownloaded?.Invoke(receiver.outputPath) == false) {

					updating = false;
					this.Dispatcher.Invoke(() => {
						Text_Message.Text = "Patching failed";
					});

				} else {
					updating = false;
					this.Dispatcher.Invoke(() => {
						Text_Message.Text = "Patching...";
					});
				}

				Thread.Sleep(1000);
				this.Dispatcher.Invoke(() => { this.Close(); });
			}
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
