using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MsgPack.Serialization;

namespace ANWI.Messaging {

	public interface MessagePayload {
		// Empty
		// This is only required so MsgPack can serialize polymorphic types
	}

}
