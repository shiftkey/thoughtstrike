using System.Linq;
using System.Security.Principal;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace Ideastrike.Helpers
{
    public static class AuthExtensions
    {
        public static string GetIdentity(this HttpRequestBase request)
        {
            var ticket = FormsAuthentication.Decrypt(request.Cookies[FormsAuthentication.FormsCookieName].Value);
            return ticket.UserData;
        }

        public static string GravatarUrl(this HtmlHelper helper)
        {
            var id = GetIdentity(helper.ViewContext.RequestContext.HttpContext.Request);
            var user = MvcApplication.UserRepository.GetUserFromUserIdentity(id);
            if (user == null) return "";
            return user.Email.ToGravatarUrl();
        }

        public static bool IsInRole(this HtmlHelper helper, string role)
        {
            var id = GetIdentity(helper.ViewContext.RequestContext.HttpContext.Request);
            var user = MvcApplication.UserRepository.GetUserFromUserIdentity(id);
            if (user == null) return false;
            var actualRoles = user.Claims.ToList();
            return actualRoles.Contains(role);
        }

        public static string ToGravatarUrl(this string emailAddress, int? size = 80)
        {
            var textToParse = string.IsNullOrWhiteSpace(emailAddress) ? "" : emailAddress.ToLower();
            var s = new System.Text.StringBuilder();
            using (var x = new System.Security.Cryptography.MD5CryptoServiceProvider())
            {
                var bs = System.Text.Encoding.UTF8.GetBytes(textToParse);
                bs = x.ComputeHash(bs);
                foreach (var b in bs)
                {
                    s.Append(b.ToString("x2").ToLower());
                }
            }
            return string.Format("http://www.gravatar.com/avatar/{0}?s={1}", s, size);
        }
    }
}