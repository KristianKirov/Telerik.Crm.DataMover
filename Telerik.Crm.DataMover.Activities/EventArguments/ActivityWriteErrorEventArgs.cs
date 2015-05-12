using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Telerik.Crm.DataMover.Activities.EventArguments
{
	public class ActivityWriteErrorEventArgs : EventArgs
	{
		public int ActivityId { get; private set; }

		public Exception Exception { get; set; }

		public ActivityWriteErrorEventArgs(int activityId, Exception exception)
		{
			this.ActivityId = activityId;
			this.Exception = exception;
		}
	}
}
