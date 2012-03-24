namespace Ideastrike.Models.Repositories
{
    public interface IActivityRepository : IGenericRepository<Activity>
    {
        bool Add(int ideaid, Activity activity);
    }
}