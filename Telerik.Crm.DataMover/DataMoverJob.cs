using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Bytes2you.Validation;

namespace Telerik.Crm.DataMover
{
	public class DataMoverJob<TKey, TData> : IDataMoverJob
	{
		private readonly IDataReader<TKey, TData> dataReader;

		private readonly IDataWriter<TData> dataWriter;

		public DataMoverJob(IDataReader<TKey, TData> dataReader, IDataWriter<TData> dataWriter)
		{
			this.dataReader = dataReader;
			this.dataWriter = dataWriter;
		}

		public async Task StartMove()
		{
			TData[] itemsToWrite = await this.dataReader.ReadNextPage();
			Guard.WhenArgument(itemsToWrite, "itemsToWrite").IsNull().Throw();

			while (itemsToWrite.Length > 0)
			{
				Task<TData[]> nextPageTask = this.dataReader.ReadNextPage();

				await this.dataWriter.WriteItems(itemsToWrite).ConfigureAwait(false);

				itemsToWrite = await nextPageTask.ConfigureAwait(false);
			}
		}
	}
}
