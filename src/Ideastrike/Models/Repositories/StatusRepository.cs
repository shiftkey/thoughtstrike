namespace Ideastrike.Models.Repositories
{
    public class StatusRepository : GenericRepository<IdeastrikeContext, Status>, IStatusRepository
    {
        public StatusRepository(IdeastrikeContext ctx) : base(ctx) { }
    }
}