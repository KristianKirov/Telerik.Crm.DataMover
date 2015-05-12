using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Telerik.Crm.DataMover.Activities.EventArguments
{
	public class ActivitiesPageReadEventArgs : EventArgs
	{
		public int From { get; private set; }

		public int To { get; private set; }

		public ActivitiesPageReadEventArgs(int from, int to)
		{
			this.From = from;
			this.To = to;
		}
	}
}
