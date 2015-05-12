using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telerik.Crm.DataMover.Activities.Model;
using Telerik.Crm.DataMover.Activities.Model.Configuration;

namespace Telerik.Crm.DataMover.Activities.Sql
{
	public class ActivitiesShortDescriptionWriter : BaseActivityWriter
	{
		private readonly RelaxOptions relax;
		private readonly IHtmlStripper htmlStripper;
		private readonly ISummaryGenerator summaryGenerator;
		private readonly IActivityProvider activityProvider;

		public ActivitiesShortDescriptionWriter(RelaxOptions relax, IHtmlStripper htmlStripper, ISummaryGenerator summaryGenerator, IActivityProvider activityProvider)
		{
			this.relax = relax;
			this.htmlStripper = htmlStripper;
			this.summaryGenerator = summaryGenerator;
			this.activityProvider = activityProvider;
		}

		public override async Task WriteItems(Activity[] items)
		{
			int itemsToRelax = this.relax.RelaxOnEach;
			foreach (Activity activity in items)
			{
				if (string.IsNullOrWhiteSpace(activity.Description))
				{
					continue;
				}

				if (itemsToRelax == 0)
				{
					itemsToRelax = this.relax.RelaxOnEach;

					await Task.Delay(this.relax.RelaxForMilliseconds);
				}
				else
				{
					--itemsToRelax;
				}

				try
				{
					string strippedDescription = htmlStripper.Strip(activity.Description);
					string summarizedDescription = summaryGenerator.GenerateSummary(strippedDescription);

					await this.activityProvider.SetShortDescription(activity.Id, summarizedDescription);

					this.OnItemWritten(activity);
				}
				catch (Exception ex)
				{
					this.OnItemFailed(activity, ex);
				}
			}
		}
	}
}
