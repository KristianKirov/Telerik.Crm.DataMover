using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Telerik.Crm.DataMover
{
	public interface IDataReader<TKey, TData>
	{
		Task<TData[]> ReadNextPage();

		Task<bool> HasMorePages();
	}
}
