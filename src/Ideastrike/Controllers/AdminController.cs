using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using Ideastrike.Helpers;
using Ideastrike.Helpers.Attributes;
using Ideastrike.Models;
using Ideastrike.Models.Repositories;
using Ideastrike.Models.ViewModels;
using Newtonsoft.Json;

namespace Ideastrike.Controllers
{
    [IdeastrikeAuthorize(Roles = "admin")]
    public class AdminController : Controller
    {
        private readonly IdeastrikeContext _dbContext;
        private ISettingsRepository _settings;
        private IUserRepository _users;
        private IIdeaRepository _ideas;
        private IActivityRepository _activities;

        public AdminController(IdeastrikeContext dbContext, ISettingsRepository settings, IUserRepository users, IIdeaRepository ideas, IActivityRepository activities)
        {
            _dbContext = dbContext;
            _settings = settings;
            _users = users;
            _ideas = ideas;
            _activities = activities;
        }

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Users()
        {
            return View(_users.GetAll());
        }

        public ActionResult User(Guid id)
        {
            var user = _users.Get(id);
            if (user == null)
            {
                //user can't be found, throw 404
                return null;
            }

            //give them a bigger gravatar picture...
            user.AvatarUrl = user.Email.ToGravatarUrl(180);
            
            return View(user);
        }

        [HttpPost]
        public ActionResult User(User user)
        {
            return RedirectToAction("User", new { id = user.Id });
        }

        public ActionResult Moderation()
        {
            return View();
        }

        public ActionResult Settings()
        {
            return View(new SettingsViewModel(_settings));
        }

        [HttpPost]
        public ActionResult Settings(SettingsViewModel settings)
        {
            _settings.WelcomeMessage = settings.WelcomeMessage;
            _settings.SiteTitle = settings.SiteTitle;
            _settings.Name = settings.Name;
            _settings.HomePage = settings.HomePage;
            _settings.GAnalyticsKey = settings.GAnalyticsKey;
            _settings.MaxThumbnailWidth = settings.MaxThumbnailWidth;
            return Settings();
        }

        public ActionResult Uservoice()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Uservoice(string channel, string forumid, string apikey, bool trusted)
        {
            var client = new WebClient();
            var suggestions = GetSuggestions(client, channel, forumid, apikey, trusted);

            foreach (var s in suggestions)
            {
                string title = s.title;

                //If the idea exists, skip it
                if (_ideas.FindBy(i => i.Title == title).Any())
                    continue;

                string date = s.created_at;
                var idea = new Idea
                {
                    Title = title,
                    Description = s.text,
                    Time = DateTime.Parse(date.Substring(0, date.Length - 5)),
                };

                string status = string.Empty;
                switch ((string)s.state)
                {
                    case "approved":
                        status = "Active";
                        break;
                    case "closed":
                        status = s.status.key == "completed" ? "Completed" : "Declined";
                        break;
                    default:
                        status = "New";
                        break;
                }
                idea.Status = status;

                //Get the author, or create
                string name = s.creator.name;
                var existing = _users.FindBy(u => u.UserName == name).FirstOrDefault();
                if (existing != null)
                    idea.Author = existing;
                else
                {
                    idea.Author = NewUser(s.creator);
                    _users.Add(idea.Author);
                }

                _ideas.Add(idea);

                //Process all comments
                var comments = GetComments(client, (string)s.id, channel, forumid, apikey, trusted);
                foreach (var c in comments)
                {
                    string commentdate = c.created_at;
                    var comment = new Comment
                    {
                        Time = DateTime.Parse(commentdate),
                        Text = c.text
                    };

                    string commentname = c.creator.name;
                    existing = _users.FindBy(u => u.UserName == commentname).FirstOrDefault();
                    if (existing != null)
                        comment.User = existing;
                    else
                    {
                        comment.User = NewUser(c.creator);
                        _users.Add(comment.User);
                    }

                    _activities.Add(idea.Id, comment);
                }

                //Process all votes
                var votes = GetVotes(client, (string)s.id, channel, forumid, apikey, trusted);
                foreach (var v in votes)
                {
                    string votername = v.user.name;
                    string votesfor = v.votes_for;
                    int vote;
                    if (Int32.TryParse(votesfor, out vote))
                    {
                        existing = _users.FindBy(u => u.UserName == votername).FirstOrDefault();
                        if (existing != null)
                            _ideas.Vote(idea.Id, existing.Id, vote);
                        else
                        {
                            var author = NewUser(v.user);
                            _users.Add(author);
                            _ideas.Vote(idea.Id, author.Id, vote);
                        }
                    }
                }
            }

            return Redirect("/admin");
        }

        private static User NewUser(dynamic user)
        {
            var author = new User
            {
                Id = Guid.NewGuid(),
                UserName = user.name,
            };

            if (user.avatar_url != null)
            {
                string avatar = user.avatar_url;
                if (avatar.Contains("&"))
                    avatar = avatar.Substring(0, avatar.IndexOf("&"));
                author.AvatarUrl = avatar;
            }
            return author;
        }

        private static IEnumerable<dynamic> GetSuggestions(WebClient client, string SuggestionsChannel, string ForumID, string APIKey, bool trusted)
        {
            var url = new Uri(string.Format("http://{0}.uservoice.com/api/v1/forums/{1}/suggestions.json?per_page=100&sort=newest&client={2}{3}", SuggestionsChannel, ForumID, APIKey, (trusted) ? "&filter=all" : ""));
            var uvResponse = client.DownloadString(url);
            var suggestions = JsonConvert.DeserializeObject<dynamic>(uvResponse).suggestions;

            return suggestions;
        }

        private static IEnumerable<dynamic> GetComments(WebClient client, string IdeadId, string SuggestionsChannel, string ForumID, string APIKey, bool trusted)
        {
            var uvResponse = client.DownloadString(new Uri(string.Format("http://{0}.uservoice.com/api/v1/forums/{1}/suggestions/{3}/comments.json?per_page=50&client={2}{4}", SuggestionsChannel, ForumID, APIKey, IdeadId, (trusted) ? "&filter=all" : "")));
            var comments = JsonConvert.DeserializeObject<dynamic>(uvResponse).comments;

            return comments;
        }

        private static IEnumerable<dynamic> GetVotes(WebClient client, string IdeadId, string SuggestionsChannel, string ForumID, string APIKey, bool trusted)
        {
            var uvResponse = client.DownloadString(new Uri(string.Format("http://{0}.uservoice.com/api/v1/forums/{1}/suggestions/{3}/supporters.json?per_page=50&client={2}{4}", SuggestionsChannel, ForumID, APIKey, IdeadId, (trusted) ? "&filter=all" : "")));
            var supporters = JsonConvert.DeserializeObject<dynamic>(uvResponse).supporters;

            return supporters;
        }
    }
}
