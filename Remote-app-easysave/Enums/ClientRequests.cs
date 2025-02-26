using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Remote_app_easysave.Enums
{
	public enum ClientRequests
	{
		Disconnect,
		Save_start,
		Save_pause,
		Save_resume,
		Save_cancel,
		Saves_request,
		Update_saves__request
	}
}
