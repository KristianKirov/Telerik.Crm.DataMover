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
		private readonly Regex removeCommentsRegex = new Regex("<!--.+?-->", RegexOptions.Singleline | RegexOptions.Compiled);

		private readonly Regex removeStylesRegex = new Regex("<style.+?</style>", RegexOptions.Singleline | RegexOptions.Compiled);

		private readonly Regex removeScriptsRegex = new Regex("<script.+?</script>", RegexOptions.Singleline | RegexOptions.Compiled);

		private readonly Regex removeSinglelineScriptsRegex = new Regex("<script.+?/>", RegexOptions.Singleline | RegexOptions.Compiled);

		private readonly Regex removeTagsRegex = new Regex("<.+?>|&nbsp;", RegexOptions.Singleline | RegexOptions.Compiled);

		private readonly Regex removeWhitespacesRegex = new Regex("\\s{2,}|\\n|\\r", RegexOptions.Compiled);

		public string Strip(string html)
		{
			string strippedHtml = this.removeCommentsRegex.Replace(html, string.Empty);
			strippedHtml = this.removeStylesRegex.Replace(strippedHtml, string.Empty);
			strippedHtml = this.removeScriptsRegex.Replace(strippedHtml, string.Empty);
			strippedHtml = this.removeSinglelineScriptsRegex.Replace(strippedHtml, string.Empty);
			strippedHtml = this.removeTagsRegex.Replace(strippedHtml, string.Empty);
			strippedHtml = this.removeWhitespacesRegex.Replace(strippedHtml, " ");

			return strippedHtml;
		}
	}
}
