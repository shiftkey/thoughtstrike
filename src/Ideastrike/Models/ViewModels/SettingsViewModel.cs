using Ideastrike.Models.Repositories;

namespace Ideastrike.Models.ViewModels
{
    public class SettingsViewModel
    {
        public string SiteTitle { get; set; }
        public string Name { get; set; }
        public string WelcomeMessage { get; set; }
        public string HomePage { get; set; }
        public string GAnalyticsKey { get; set; }
        public int MaxThumbnailWidth { get; set; }

        public SettingsViewModel()
        {
            
        }

        public SettingsViewModel(ISettingsRepository settings)
        {
            SiteTitle = settings.SiteTitle;
            Name = settings.Name;
            WelcomeMessage = settings.WelcomeMessage;
            HomePage = settings.HomePage;
            GAnalyticsKey = settings.GAnalyticsKey;
            MaxThumbnailWidth = settings.MaxThumbnailWidth;
        }
    }
}