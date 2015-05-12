using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Bytes2you.Validation;
using Telerik.Crm.DataMover.Activities.Model;
using Telerik.Crm.DataMover.Activities.Sql.Data;

namespace Telerik.Crm.DataMover.Activities.Sql
{
	public class SqlActivitiesReader : IDataReader<int, Activity>
	{
		private readonly int endKeyInclusive;

		private readonly int pageSize;

		private int startKey;

		private readonly IActivityProvider activityProvider;

		public SqlActivitiesReader(int startKeyInclusive, int endKeyInclusive, int pageSize, IActivityProvider activityProvider)
		{
			this.startKey = startKeyInclusive;
			this.endKeyInclusive = endKeyInclusive;
			this.pageSize = pageSize;

			this.activityProvider = activityProvider;
		}

		public async Task<Activity[]> ReadNextPage()
		{
			Activity[] readActivities = await this.activityProvider.GetOrderedById(this.startKey, this.endKeyInclusive, this.pageSize).ConfigureAwait(false);
			Guard.WhenArgument(readActivities, "readActivities").IsNull().Throw();

			if (readActivities.Length == 0)
			{
				return readActivities;
			}

			this.startKey = readActivities[readActivities.Length - 1].Id + 1;

			return readActivities;
		}
	}
}
