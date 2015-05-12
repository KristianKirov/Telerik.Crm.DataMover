using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Telerik.Crm.DataMover
{
	public interface IParallelReadersFactory<TKey, TData>
	{
		Task<IDataReader<TKey, TData>[]> GetParallelReaders();
	}
}
