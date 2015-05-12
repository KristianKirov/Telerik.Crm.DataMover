using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telerik.Crm.DataMover.Activities.Model;

namespace Telerik.Crm.DataMover.Activities.Sql
{
	public class SqlActivitiesParallelReadersFactory : IParallelReadersFactory<int, Activity>
	{
		private readonly IActivityProvider activityProvider;
		private readonly int readersCount;
		private readonly int pageSize;

		public SqlActivitiesParallelReadersFactory(IActivityProvider activityProvider, int readersCount, int pageSize)
		{
			this.activityProvider = activityProvider;
			this.readersCount = readersCount;
			this.pageSize = pageSize;
		}

		public async Task<IDataReader<int, Activity>[]> GetParallelReaders()
		{
			int activitiesCount = await this.activityProvider.GetTotalItemsCount().ConfigureAwait(false);
			int readersPageSize = (activitiesCount + this.readersCount - 1) / this.readersCount;

			IDataReader<int, Activity>[] readers = new IDataReader<int, Activity>[readersCount];
			for (int readerId = 0; readerId < this.readersCount; readerId++)
			{
				int startKey = readerId * readersPageSize;
				int endKey = readerId == (readersCount - 1) ? int.MaxValue : ((readerId + 1) * readersPageSize) - 1;

				readers[readerId] = new SqlActivitiesReader(startKey, endKey, this.pageSize, this.activityProvider);
			}

			return readers;
		}
	}
}
