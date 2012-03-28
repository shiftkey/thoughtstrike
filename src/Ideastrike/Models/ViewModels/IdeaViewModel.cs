using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using DanTup.Web;
using Ideastrike.Helpers;

namespace Ideastrike.Models.ViewModels
{
	public class IdeaViewModel
	{
		public IdeaViewModel(Idea idea)
		{
            var regex = new Regex(@"((https?|ftp|file):\/\/[-a-zA-Z0-9+&@#\/%?=~_|!:,.;]*[-a-zA-Z0-9+&@#\/%=~_|])");
            
            Id = idea.Id;
			Title = idea.Title;
			Status = idea.Status;
			Time = idea.Time.ToFriendly();
		    var text = regex.Replace(idea.Description, @"[$1]($1)");
            Description = MarkdownHelper.Markdown(text);
			UserHasVoted = idea.UserHasVoted;
			TotalVotes = idea.Votes.Count;
			Author = idea.Author;
			GravatarUrl = (string.IsNullOrEmpty(Author.AvatarUrl)) ? Author.Email.ToGravatarUrl(40) : Author.AvatarUrl;


            Features = idea.Features.Select(f => new FeatureViewModel(f)).ToList();

			Activities = idea.Activities.Select(f => new ActivityViewModel(f)).ToList();

			if (idea.Images != null)
			{
				Images = idea.Images.ToList();
			}
		}

		public IEnumerable<Image> Images { get; set; }

		public IEnumerable<FeatureViewModel> Features { get; set; }
		
		[Obsolete("Make a secondary call to fetch these and render dynamically")]
		public IEnumerable<ActivityViewModel> Activities { get; set; }

		public string GravatarUrl { get; private set; }
        public IHtmlString Time { get; private set; }
		public User Author { get; private set; }
		public bool UserHasVoted { get; set; }
		public int TotalVotes { get; private set; }
		public string Title { get; private set; }
		public string Status { get; private set; }
		public int Id { get; private set; }
		public IHtmlString Description { get; private set; }
	}
}