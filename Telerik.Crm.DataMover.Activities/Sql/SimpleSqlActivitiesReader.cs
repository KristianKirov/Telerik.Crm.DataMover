using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telerik.Crm.DataMover.Activities.EventArguments;
using Telerik.Crm.DataMover.Activities.Model;

namespace Telerik.Crm.DataMover.Activities.Sql
{
	public class SimpleSqlActivitiesReader : IDataReader<int, Activity>
	{
		private int pageStartId;

		private Activity[] lastReadItems;

		private readonly int pageSize;

		private readonly IActivityProvider activityProvider;

		private readonly int doNotStopBeforeId;

		public event EventHandler<ActivitiesPageReadEventArgs> PageRead;

		public SimpleSqlActivitiesReader(int startId, int pageSize, IActivityProvider activityProvider, int doNotStopBeforeId)
		{
			this.pageStartId = startId;
			this.pageSize = pageSize;
			this.activityProvider = activityProvider;
			this.doNotStopBeforeId = doNotStopBeforeId;
		}

		public async Task<Activity[]> ReadNextPage()
		{
			int nextPageStartId = this.pageStartId + this.pageSize;
			int pageEndId = nextPageStartId - 1;
			Activity[] activitiesInRange = await this.activityProvider.GetInRange(this.pageStartId, pageEndId);

			if (this.PageRead != null)
			{
				this.PageRead(this, new ActivitiesPageReadEventArgs(this.pageStartId, pageEndId));
			}

			this.pageStartId = nextPageStartId;

			this.lastReadItems = activitiesInRange;

			return activitiesInRange;
		}

		public Task<bool> HasMorePages()
		{
			bool result;
			if (this.pageStartId <= this.doNotStopBeforeId)
			{
				result = true;
			}
			else
			{
				result = this.lastReadItems == null ? true : this.lastReadItems.Length > 0;
			}

			return Task.FromResult(result);
		}
	}
}
