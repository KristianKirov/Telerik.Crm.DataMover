using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telerik.Crm.DataMover.Activities.Model;

namespace Telerik.Crm.DataMover.Activities.Text
{
	public class FixedLengthSummaryGenerator : ISummaryGenerator
	{
		private readonly int maxSummarylength;

		public FixedLengthSummaryGenerator(int maxSummarylength)
		{
			this.maxSummarylength = maxSummarylength;
		}

		public string GenerateSummary(string text)
		{
			if (text.Length <= this.maxSummarylength)
			{
				return text;
			}

			return text.Substring(0, maxSummarylength);
		}
	}
}
