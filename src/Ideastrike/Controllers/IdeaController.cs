using System;
using System.Linq;
using System.Web.Mvc;
using System.Web.Security;
using Ideastrike.Helpers;
using Ideastrike.Helpers.Attributes;
using Ideastrike.Models;
using Ideastrike.Models.Repositories;
using Ideastrike.Models.ViewModels;

namespace Ideastrike.Controllers
{
    public class IdeaController : Controller
    {
        private readonly IIdeaRepository _ideas;
        private readonly IUserRepository _users;
        private readonly ISettingsRepository _settings;

        public IdeaController(IIdeaRepository ideas, IUserRepository users, ISettingsRepository settings, IImageRepository imageRepository)
        {
            _ideas = ideas;
            _users = users;
            _settings = settings;
        }

        public ActionResult Index(int id)
        {
            var idea = _ideas.Get(id);
            ViewBag.Title = string.Format("{0} - {1}", idea.Title, _settings.SiteTitle);
            ViewBag.CanChangeIdeaStatus = false;
            ViewBag.StatusChoices = _settings.IdeaStatusChoices.Split(',');
            ViewBag.UserHasVoted = false;

            if (User.Identity.IsAuthenticated)
            {
                var user = _users.GetUserFromUserIdentity(Request.GetIdentity());
                if (idea.Votes.Any(u => u.UserId == user.Id))
                    ViewBag.UserHasVoted = true;

                ViewBag.CanChangeIdeaStatus = user.Claims.Contains("admin");
            }
            return View(new IdeaViewModel(idea));

        }

        [IdeastrikeAuthorize]
        public ActionResult New()
        {
            ViewBag.Errors = false;
            ViewBag.Ideas = _ideas.GetAll().OrderByDescending(i => i.Time).Take(5);
            return View(new Idea());
        }

        [HttpPost]
        [IdeastrikeAuthorize]
        public ActionResult New(Idea idea)
        {

            var user = _users.GetUserFromUserIdentity(Request.GetIdentity());

            if (user == null)
                return Redirect("/");

            idea.Author = user;
            idea.Time = DateTime.UtcNow;
            idea.Status = _settings.IdeaStatusDefault;


            //IEnumerable<string> keys = Context.Request.Form;

            //var parameters = keys.Where(c => c.StartsWith("imageId"));
            //var ids = parameters.Select(c => Context.Request.Form[c].ToString()).Cast<string>();
            //var images = ids.Select(id => _imageRepository.Get(Convert.ToInt32(id)));
            //idea.Images = images.ToList();

            if (idea.Votes.Any(u => u.UserId == user.Id))
                idea.UserHasVoted = true;

            _ideas.Add(idea);

            return RedirectToAction("Index", new { id = idea.Id });
        }

        [HttpGet]
        public JsonResult Activity(int id)
        {
            var idea = _ideas.Get(id);
            var results = idea.Activities.OrderBy(a => a.Time).Select(MapToViewModel);
            return new JsonResult
                       {
                           Data = new
                                      {
                                          Status = "success",
                                          Items = results
                                      },
                           JsonRequestBehavior = JsonRequestBehavior.AllowGet
                       };
        }

        [HttpPost]
        public JsonResult Vote(int id)
        {
            if (!User.Identity.IsAuthenticated)
            {
                //http://weblogs.asp.net/jgalloway/archive/2012/03/23/asp-net-web-api-screencast-series-part-6-authorization.aspx
                /*
                if (user == null)
                    return Response.AsRedirect("/login");
                */
            }

            var user = _users.GetUserFromUserIdentity(Request.GetIdentity());
            int votes = _ideas.Vote(id, user.Id, 1);

            return new JsonResult
                       {
                           Data = new { Status = "OK", NewVotes = votes }
                       };
        }
        [HttpPost]
        public JsonResult Unvote(int id)
        {
            var user = _users.GetUserFromUserIdentity(Request.GetIdentity());
            int votes = _ideas.Unvote(id, user.Id);
            return new JsonResult
                       {
                           Data = new { Status = "OK", NewVotes = votes }
                       };
        }

        private static object MapToViewModel(Activity activity)
        {
            var github = activity as GitHubActivity;

            if (github != null)
                return new { template = "github", item = new GitHubActivityViewModel(github) };

            var comment = activity as Comment;
            if (comment != null)
                return new { template = "comment", item = new CommentViewModel(comment) };

            var admin = activity as AdminActivity;
            if (admin != null)
                return new { template = "admin", item = new AdminActivityViewModel(admin) };

            return null;
        }
    }
}
