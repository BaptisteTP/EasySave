using Remote_app_easysave.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Remote_app_easysave.Models
{
	public class ServerPacket
	{
		public ServerResponses ServerResponse { get; set; }
		public byte[] Payload { get; set; }
	}
}
