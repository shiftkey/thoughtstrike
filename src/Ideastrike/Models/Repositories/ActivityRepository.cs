namespace Ideastrike.Models.Repositories
{
    public class ActivityRepository : GenericRepository<IdeastrikeContext, Activity>, IActivityRepository
    {
        public ActivityRepository(IdeastrikeContext ctx) : base(ctx) { }
        
        public bool Add(int ideaid, Activity activity)
        {
            var idea = Context.Ideas.Find(ideaid);
            if (idea == null)
                return false;

            Context.Users.Attach(activity.User);
            Context.Ideas.Attach(idea);

            idea.Activities.Add(activity);
            Context.SaveChanges();
            return true;
        }
    }
}