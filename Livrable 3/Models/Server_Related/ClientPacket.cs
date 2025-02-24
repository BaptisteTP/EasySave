using EasySave2._0.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace c.Models.Server_Related
{
	public class ClientPacket
	{
		public ClientRequests ClientRequest { get; set; }
		public byte[]? Payload { get; set; }
	}
}
