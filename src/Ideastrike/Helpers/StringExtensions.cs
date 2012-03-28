using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;

namespace Ideastrike.Helpers
{
    public static class StringExtensions
    {
        public static IHtmlString ToHtmlString(this string s)
        {
            return MvcHtmlString.Create(s);
        }

        public static string ConvertingLinksToMarkdownUrls(this string s)
        {
            var regex = new Regex(@"((https?|ftp|file):\/\/[-a-zA-Z0-9+&@#\/%?=~_|!:,.;]*[-a-zA-Z0-9+&@#\/%=~_|])");

            return regex.Replace(s, @"[$1]($1)");
        }
    }
}