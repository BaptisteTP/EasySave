using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasySave2._0.Enums
{
	public enum ServerResponses
	{
		Send_save,

		Save_created,
		Save_edited,
		Save_deleted,
		Save_started,
		Save_finished,
		Save_progress_update,
		Save_paused,
		Save_resumed,
		Save_stopped,

		Save_already_started,
		Save_already_paused,
		Save_already_resumed,
		Save_already_canceled,

		Cannot_resume_save,

		Save_error_detected,
		Save_waiting_crtical_files_copy,
		Save_copy_critical_files_ended,
		
	}
}
