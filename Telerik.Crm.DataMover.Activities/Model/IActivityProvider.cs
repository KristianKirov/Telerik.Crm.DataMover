using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Telerik.Crm.DataMover.Activities.Model
{
	public interface IActivityProvider
	{
		Task<int> GetTotalItemsCount();

		Task<Activity[]> GetOrderedById(int startKey, int endKey, int pageSize);

		Task<Activity[]> GetInRange(int fromInclusive, int toInclusive);

		Task SetShortDescription(int activityId, string shortDescription);
	}
}
