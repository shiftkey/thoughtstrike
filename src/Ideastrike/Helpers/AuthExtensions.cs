using System.Web;
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
    }
}