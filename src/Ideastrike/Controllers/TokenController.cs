using System;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using Ideastrike.Localization;
using Ideastrike.Models;
using Ideastrike.Models.Repositories;
using Newtonsoft.Json;

namespace Ideastrike.Controllers
{
    public class TokenController : Controller
    {
        private readonly IUserRepository _userRepository;
        private readonly string _apikey;

        public TokenController(IUserRepository userRepository)
        {
            _userRepository = userRepository;
            _apikey = ConfigurationManager.AppSettings["JanrainKey"];
        }

        [HttpPost]
        public ActionResult Login(string token)
        {
            if (string.IsNullOrWhiteSpace(token))
                return Error(Strings.LoginModule_BadResponse_NoToken);

            var response = new WebClient().DownloadString(string.Format("https://rpxnow.com/api/v2/auth_info?apiKey={0}&token={1}", _apikey, token));

            if (string.IsNullOrWhiteSpace(response))
                return Error(Strings.LoginModule_BadResponse_NoUser);

            var j = JsonConvert.DeserializeObject<dynamic>(response);

            if (j.stat.ToString() != "ok")
                return Error(Strings.LoginModule_BadResponse);

            var userIdentity = j.profile.identifier.ToString();
            var user = _userRepository.GetUserFromUserIdentity(userIdentity);

            if (user != null)
            {
                // we have an existing user, just log them in
                var auth = new HttpCookie(FormsAuthentication.FormsCookieName, GenerateTicket(user));
                Response.Cookies.Add(auth);
                return Redirect("/");
            }

            var username = j.profile.preferredUsername.ToString();
            var email = string.Empty;
            if (j.profile.email != null)
                email = j.profile.email.ToString();

            var u = new User
            {
                Id = Guid.NewGuid(),
                Identity = userIdentity,
                UserName = !string.IsNullOrEmpty(username) ? username : "New User " + _userRepository.GetAll().Count(),
                Email = !string.IsNullOrEmpty(email) ? email : "none@void.com",
                Github = !string.IsNullOrEmpty(username) ? username : "",
                IsActive = true,
            };

            if (!_userRepository.GetAll().Any())
                _userRepository.AddRole(u, "Admin");

            if (j.profile.photo != null)
                u.AvatarUrl = j.profile.photo.ToString();

            _userRepository.Add(u);
            // TODO: add user to forms authentication
            // TODO: navigate them to /profile/edit
      
            var cookie = new HttpCookie(FormsAuthentication.FormsCookieName,  GenerateTicket(u));

            //OM NOM NOM. ALL OF THE COOKIES
            Response.Cookies.Add(cookie);
            return View();
        }

        private ActionResult Error(string message)
        {
            ViewBag.Title = Strings.LoginModule_LoginError;
            ViewBag.Message = message;

            return View("Error");
        }

        [Authorize]
        public ActionResult Logout()
        {
            // TODO: clear forms auth
            FormsAuthentication.SignOut();
            return RedirectToAction("Index", "Home");
        }

        private string GenerateTicket(User user)
        {
            var newticket = new FormsAuthenticationTicket(
                1,
                user.UserName,
                DateTime.UtcNow,
                DateTime.UtcNow.AddMonths(1),
                true,
                user.Identity);

            return FormsAuthentication.Encrypt(newticket);
        }
    }
}
