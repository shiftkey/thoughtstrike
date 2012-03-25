using System;
using System.Linq;
using System.Web.Mvc;
using Ideastrike.Helpers;
using Ideastrike.Helpers.Attributes;
using Ideastrike.Models;
using Ideastrike.Models.Repositories;

namespace Ideastrike.Controllers
{
    public class CommentController : Controller
    {
        private readonly IIdeaRepository _ideas;
        private readonly IActivityRepository _activities;
        private readonly IUserRepository _users;

        public CommentController(IIdeaRepository ideas, IActivityRepository activities, IUserRepository users)
        {
            _ideas = ideas;
            _activities = activities;
            _users = users;
        }

        [HttpPost]
        [IdeastrikeAuthorize]
        public ActionResult Add(int id)
        {
            //TODO: Validation
            var user = _users.GetUserFromUserIdentity(Request.GetIdentity());
            var comment = new Comment
                              {
                                  User = user,
                                  Time = DateTime.UtcNow,
                                  Text = Request.Form["comment"]
                              };

            _activities.Add(id, comment);
            return Redirect(string.Format("/idea/{0}#{1}", id, comment.Id));
        }

        [HttpPost]
        [IdeastrikeAuthorize(Roles = "admin")]
        public ActionResult Admincomment(int id)
        {

            var idea = _ideas.Get(id);


            var user = _users.GetUserFromUserIdentity(Request.GetIdentity());

            _activities.Add(id, new AdminActivity
                                   {
                                       OldStatus = idea.Status,
                                       NewStatus = Request.Form["Status"],
                                       User = user,
                                       Time = DateTime.UtcNow
                                   });
            _activities.Save();

            idea.Status = Request.Form["Status"];
            _ideas.Save();

            return Redirect("/idea/" + idea.Id);
        }

    }
}
