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
    }
}