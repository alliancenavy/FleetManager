using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ANWI;

namespace Client {
	public interface IMailbox {
		void DeliverMessage(ANWI.Messaging.Message msg);
	}
}
