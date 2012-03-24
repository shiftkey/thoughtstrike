namespace Ideastrike.Models.Repositories
{
    public interface IFeatureRepository : IGenericRepository<Feature>
    {
        bool Add(int ideaid, Feature feature);
    }
}