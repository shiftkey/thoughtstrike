using System.Text.RegularExpressions;
using System.Web;
using DanTup.Web;
using Ideastrike.Helpers;

namespace Ideastrike.Models.ViewModels
{
    public class FeatureViewModel
    {
        public FeatureViewModel(Feature feature)
        {
            Text = MarkdownHelper.Markdown(feature.Text.ConvertingLinksToMarkdownUrls());
            FriendlyTime = feature.Time.ToFriendly();
            Author = feature.User.UserName;
            GravatarUrl = (string.IsNullOrEmpty(feature.User.AvatarUrl)) ? feature.User.Email.ToGravatarUrl(40) : feature.User.AvatarUrl;
        }

        public IHtmlString FriendlyTime { get; private set; }

        public IHtmlString Text { get; private set; }
        public string Author { get; private set; }
        public string GravatarUrl { get; private set; }
    }
}