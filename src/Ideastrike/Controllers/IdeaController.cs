using System.Linq;
using System.Web.Mvc;
using Ideastrike.Models;
using Ideastrike.Models.Repositories;
using Ideastrike.Models.ViewModels;

namespace Ideastrike.Controllers
{
    public class IdeaController : Controller
    {
        private readonly IIdeaRepository _ideas;
        private readonly ISettingsRepository _settings;

        public IdeaController(IIdeaRepository ideas, IUserRepository users, ISettingsRepository settings, IImageRepository imageRepository)
        {
            _ideas = ideas;
            _settings = settings;
        }

        public ActionResult Index(int id)
        {
            var idea = _ideas.Get(id);
            ViewBag.Title = string.Format("{0} - {1}", idea.Title, _settings.SiteTitle);
            ViewBag.CanChangeIdeaStatus = false;
            ViewBag.StatusChoices = _settings.IdeaStatusChoices.Split(','); ;
            /* 
                User user = Context.GetCurrentUser(_users);
                if (user != null)
                {
                    if (idea.Votes.Any(u => u.UserId == user.Id))
                        idea.UserHasVoted = true;

                }
                if (user != null) 
                {
                    model.CanChangeIdeaStatus = Context.CurrentUser.Claims.Contains("admin");
                }
            };*/

            return View(new IdeaViewModel(idea));

        }

        public ActionResult New()
        {
            ViewBag.Errors = false;
            ViewBag.Ideas = _ideas.GetAll().OrderByDescending(i => i.Time).Take(5);
            return View(new Idea());
        }

        [HttpPost]
        public ActionResult New(Idea idea)
        {
            /*   if (string.IsNullOrEmpty(Request.Form.Title) || string.IsNullOrEmpty(Request.Form.Description))
                {
                    return Response.AsRedirect("/idea/new?validation=failed");
                }

                var user = _users.FindBy(u => u.UserName == Context.CurrentUser.UserName).FirstOrDefault();

                if (user == null)
                    return Response.AsRedirect("/login");

                var idea = new Idea
                            {
                                Author = user,
                                Time = DateTime.UtcNow,
                                Title = Request.Form.Title,
                                Description = Request.Form.Description,
                                Status = settings.IdeaStatusDefault
                            };

                IEnumerable<string> keys = Context.Request.Form;

                var parameters = keys.Where(c => c.StartsWith("imageId"));
                var ids = parameters.Select(c => Context.Request.Form[c].ToString()).Cast<string>();
                var images = ids.Select(id => _imageRepository.Get(Convert.ToInt32(id)));
                idea.Images = images.ToList();

                //i.Images = form.Cast<string>()
                //    .Where(k => k.StartsWith("imageId"))
                //    .Select(k => _imageRepository.Get(Convert.ToInt32(form[k])))
                //    .ToList(); //is there a way to do this using Nancy?
                if (idea.Votes.Any(u => u.UserId == user.Id))
                    idea.UserHasVoted = true;

                ideas.Add(idea);

                return Response.AsRedirect("/idea/" + idea.Id);*/
            return RedirectToAction("Index", new { id = 1 });
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
