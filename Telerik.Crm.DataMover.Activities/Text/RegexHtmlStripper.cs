using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Telerik.Crm.DataMover.Activities.Model;

namespace Telerik.Crm.DataMover.Activities.Text
{
	public class RegexHtmlStripper : IHtmlStripper
	{
		private readonly Regex removeTagsRegex = new Regex("<[^>]+>|&nbsp;", RegexOptions.Compiled);

		private readonly Regex removeWhitespacesRegex = new Regex("\\s{2,}|\\n|\\r", RegexOptions.Compiled);

		public string Strip(string html)
		{
			string htmlWithoutTags = removeTagsRegex.Replace(html, string.Empty);
			string htmlWithoutMultipleSpacesAndNewLines = removeWhitespacesRegex.Replace(htmlWithoutTags, " ");

			return htmlWithoutMultipleSpacesAndNewLines;
		}
	}
}
