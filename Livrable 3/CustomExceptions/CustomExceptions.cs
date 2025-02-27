using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasySave2._0.CustomExceptions
{
	public class BuisnessSoftwareUpException : Exception { }
	public class CriticalFilesCopyException : Exception { }
	public class SaveNotPausedException : Exception { }
}
