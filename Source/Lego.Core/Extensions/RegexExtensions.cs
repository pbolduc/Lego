using System.Text.RegularExpressions;

namespace Lego.Extensions
{
    public static class RegexExtensions
    {
        public static string ReplaceRegex(this string value, string pattern, string replacement)
        {
            return Regex.Replace(value, pattern, replacement);
        }
    }
}
