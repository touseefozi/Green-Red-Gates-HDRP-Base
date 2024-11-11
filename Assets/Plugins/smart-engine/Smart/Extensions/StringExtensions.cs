using System.Globalization;
using System.Text.RegularExpressions;
using UnityEngine.Assertions;

namespace Smart.Basics.Extensions
{
	public static class StringExtensions
	{
		public static string GetRegexMatch(this string text, string pattern)
		{
			var regex = new Regex(pattern);
			var match = regex.Match(text);
			Assert.IsTrue(match.Success);
			return match.Value;
		}
		
		public static string ToCamelCase(this string message)
		{
			message = message.Replace("-", " ").Replace("_", " ");
			message = CultureInfo.InvariantCulture.TextInfo.ToTitleCase(message);
			message = message.Replace(" ", "");
			return message;
		}

		public static string SplitCamelCase(this string camelCaseString)
		{
			if (string.IsNullOrEmpty(camelCaseString)) return camelCaseString;

			string camelCase = Regex.Replace(Regex.Replace(camelCaseString, @"(\P{Ll})(\P{Ll}\p{Ll})", "$1 $2"), @"(\p{Ll})(\P{Ll})", "$1 $2");
			string firstLetter = camelCase.Substring(0, 1).ToUpper();

			if (camelCaseString.Length > 1)
			{
				string rest = camelCase.Substring(1);

				return firstLetter + rest;
			}

			return firstLetter;
		}
	}
}