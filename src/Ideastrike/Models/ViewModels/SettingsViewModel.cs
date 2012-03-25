using Ideastrike.Models.Repositories;

namespace Ideastrike.Models.ViewModels
{
    public class SettingsViewModel
    {
        public string SiteTitle;
        public string Name;
        public string WelcomeMessage;
        public string HomePage;
        public string GAnalyticsKey;
        public int MaxThumbnailWidth;

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