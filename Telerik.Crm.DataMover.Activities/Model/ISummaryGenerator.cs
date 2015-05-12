using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Telerik.Crm.DataMover.Activities.Model
{
	public interface ISummaryGenerator
	{
		string GenerateSummary(string text);
	}
}
