using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Telerik.Crm.DataMover.Activities.Model.Configuration
{
	public class ActivityDataConfig
	{
		public string ConnectionString { get; set; }

		public string GetPagedActivitiesByIdQuery { get; set; }

		public string GetActivitiesCountQuery { get; set; }

		public string GetActivitiesInRangeQuery { get; set; }

		public string SetActivityShortDescriptionQuery { get; set; }

		public string DisableTriggersQuery { get; set; }

		public string EnableTriggersQuery { get; set; }

		public string GetMaxActivityIdQuery { get; set; }
	}
}
