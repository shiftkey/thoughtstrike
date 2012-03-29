using System.Linq;
using System.Web.Mvc;
using Ideastrike.Helpers;
using Ideastrike.Helpers.Attributes;
using Ideastrike.Localization;
using Ideastrike.Models.Repositories;

namespace Ideastrike.Controllers
{
    [IdeastrikeAuthorize]
    public class ProfileController : Controller
    {
        private readonly IUserRepository _users;
        private readonly IIdeaRepository _ideas;
        private readonly IFeatureRepository _features;

        public ProfileController(IUserRepository users, IIdeaRepository ideas, IFeatureRepository features)
        {
            _users = users;
            _ideas = ideas;
            _features = features;
        }


        public ActionResult Index()
        {
            var user = _users.GetUserFromUserIdentity(Request.GetIdentity());
            var usersIdeas = _ideas.GetAll().Where(u => u.Author.Id == user.Id).ToList();
            var usersFeatures = _features.GetAll().Where(u => u.User.Id == user.Id).ToList();
            var usersVotes = _users.GetVotes(user.Id).ToList();

            return View(new
                            {
                                Ideas = usersIdeas,
                                Features = usersFeatures,
                                Votes = usersVotes,
                                Id = user.Id,
                                UserName = user.UserName,
                                Email = user.Email,
                                Github = user.Github,
                                AvatarUrl = user.AvatarUrl,
                            }.ToExpando());
        }

        public ActionResult Edit()
        {
            var user = _users.GetUserFromUserIdentity(Request.GetIdentity());

            return View(new
            {
                UserName = user.UserName,
                Email = user.Email,
                Github = user.Github,
            }.ToExpando());
        }

        [HttpPost]
        public ActionResult Edit(string username, string email, string github)
        {
            var user = _users.GetUserFromUserIdentity(Request.GetIdentity());
            user.UserName = username;
            user.Email = email;
            user.AvatarUrl = email.ToGravatarUrl(40);
            user.Github = github;

            _users.Edit(user);

            return Redirect("/profile");
        }

        [HttpGet]
        public JsonResult CheckUser(string username)
        {
            bool userExists = _users.FindBy(u => u.UserName == username).Any();

            var msg = "";
            if (username == User.Identity.Name)
                msg = "";
            else if (string.IsNullOrWhiteSpace(username))
                msg = Strings.UserModule_UsernameNotValid;
            else if (userExists)
                msg = Strings.UserModule_UsernameTaken;
            else
                msg = Strings.UserModule_UsernameAvailable;

            return new JsonResult
            {
                Data = new
                {
                    Status = "OK",
                    msg = msg
                },
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }
    }
}
