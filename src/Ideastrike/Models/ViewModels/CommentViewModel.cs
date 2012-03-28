using System.Text.RegularExpressions;
using Ideastrike.Helpers;

namespace Ideastrike.Models.ViewModels
{
    public class CommentViewModel
    {
        public CommentViewModel(Comment comment)
        {
            var regex = new Regex(@"((https?|ftp|file):\/\/[-a-zA-Z0-9+&@#\/%?=~_|!:,.;]*[-a-zA-Z0-9+&@#\/%=~_|])");

            FriendlyTime = comment.Time.ToFriendly().ToHtmlString(); // this is encoding when it shouldn't be

            var text = regex.Replace(comment.Text, @"[$1]($1)");
            Text = text;

            Author = comment.User.UserName;
            GravatarUrl = (string.IsNullOrEmpty(comment.User.AvatarUrl)) ? comment.User.Email.ToGravatarUrl(40) : comment.User.AvatarUrl;
        }

        public string FriendlyTime { get; private set; }

        public string Text { get; private set; }
        public string Author { get; private set; }
        public string GravatarUrl { get; private set; }
    }
}