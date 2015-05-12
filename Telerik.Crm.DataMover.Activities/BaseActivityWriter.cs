using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telerik.Crm.DataMover.Activities.EventArguments;

namespace Telerik.Crm.DataMover.Activities
{
	public abstract class BaseActivityWriter : IDataWriter<Activity>
	{
		public event EventHandler<ActivityWrittenEventArgs> ItemWritten;

		public event EventHandler<ActivityWriteErrorEventArgs> ItemFailed;

		protected void OnItemWritten(Activity activity)
		{
			if (this.ItemWritten != null)
			{
				this.ItemWritten(this, new ActivityWrittenEventArgs(activity));
			}
		}

		protected void OnItemFailed(Activity activity, Exception exception)
		{
			if (this.ItemFailed != null)
			{
				this.ItemFailed(this, new ActivityWriteErrorEventArgs(activity.Id, exception));
			}
		}

		public abstract Task WriteItems(Activity[] activities);
	}
}
