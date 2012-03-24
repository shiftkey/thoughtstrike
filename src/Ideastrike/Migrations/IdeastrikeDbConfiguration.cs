using System.Data.Entity.Migrations;
using Ideastrike.Models;
using System.Linq;

namespace Ideastrike.Migrations
{
    internal sealed class IdeastrikeDbConfiguration : DbMigrationsConfiguration<IdeastrikeContext>
    {
        private const string IdeaStatusDefault = "New";

        public IdeastrikeDbConfiguration()
        {
            AutomaticMigrationsEnabled = false;
        }

        protected override void Seed(IdeastrikeContext context)
        {
            if (!context.Settings.Any())
            {
                context.Settings.AddOrUpdate(s => s.Key, new Setting { Key = "Title", Value = "Yet Another Ideastrike" });
                context.Settings.AddOrUpdate(s => s.Key, new Setting { Key = "Name", Value = "Ideastrike" });
                context.Settings.AddOrUpdate(s => s.Key, new Setting { Key = "WelcomeMessage", Value = "You've been.... Ideastruck" });
                context.Settings.AddOrUpdate(s => s.Key, new Setting { Key = "HomePage", Value = "http://www.code52.org" });
                context.Settings.AddOrUpdate(s => s.Key, new Setting { Key = "GAnalyticsKey", Value = "" });
                context.Settings.AddOrUpdate(s => s.Key, new Setting { Key = "IdeaStatusChoices", Value = "New,Active,Completed,Declined" });
                context.Settings.AddOrUpdate(s => s.Key, new Setting { Key = "IdeaStatusDefault", Value = IdeaStatusDefault });
                context.SaveChanges();
            }

            if (context.Settings.FirstOrDefault<Setting>(s => s.Key == "MaxThumbnailWidth") == null)
            {
                context.Settings.AddOrUpdate(s => s.Key, new Setting { Key = "MaxThumbnailWidth", Value = 1000.ToString() });
            }

            context.Claims.AddOrUpdate(s => s.Name, new Claim { Name = "admin"});
            context.Claims.AddOrUpdate(s => s.Name, new Claim { Name = "moderator" });
        }
    }
}
