using System.Linq;
using System.Text.RegularExpressions;

namespace Smart.Extensions
{
	public static class RegexExtensions
	{
		public static string KeepMatching(this Regex regex, string input)
		{
			return regex.Matches(input).Cast<Match>().Aggregate(string.Empty, (a, m) => a + m.Value);	
		}
	}
}