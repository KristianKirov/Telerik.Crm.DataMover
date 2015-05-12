using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Bytes2you.Validation;

namespace Telerik.Crm.DataMover
{
	public class ParallelDataMoverJob<TKey, TData> : IDataMoverJob
	{
		private readonly IParallelReadersFactory<TKey, TData> readersFactory;

		private readonly IDataWriter<TData> dataWriter;

		public ParallelDataMoverJob(IParallelReadersFactory<TKey, TData> readersFactory, IDataWriter<TData> dataWriter)
		{
			this.dataWriter = dataWriter;
			this.readersFactory = readersFactory;
		}
		
		public async Task StartMove()
		{
			IDataReader<TKey, TData>[] readers = await this.readersFactory.GetParallelReaders().ConfigureAwait(false);
			Guard.WhenArgument(readers, "readers").IsNull().Throw();

			DataMoverJob<TKey, TData>[] parallelJobs = readers.Select(r => new DataMoverJob<TKey, TData>(r, this.dataWriter)).ToArray();

			List<Task> parallelTasks = new List<Task>();
			foreach (DataMoverJob<TKey, TData> parallelJob in parallelJobs)
			{
				Task moveTask = parallelJob.StartMove();
				parallelTasks.Add(moveTask);
			}

			await Task.WhenAll(parallelTasks.ToArray()).ConfigureAwait(false);
		}
	}
}
