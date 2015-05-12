using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Telerik.Crm.DataMover.Activities.EventArguments
{
	public class ActivityWrittenEventArgs : EventArgs
	{
		public Activity Activity { get; private set; }

		public ActivityWrittenEventArgs(Activity activity)
		{
			this.Activity = activity;
		}
	}
}
