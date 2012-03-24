using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Ideastrike.Models.Repositories
{
    public interface IUserRepository
    {
        //IUserIdentity GetUserFromIdentifier(Guid identifier);

        User GetUserFromUserIdentity(string identity);

        IQueryable<User> GetAll();
        IQueryable<User> FindBy(Expression<Func<User, bool>> predicate);
        User Get(Guid id);
        void Add(User entity);
        void Delete(Guid id);
        void Edit(User entity);
        void Save();
        void AddRole(User entity, string roleName);
        ICollection<Vote> GetVotes(Guid id);
    }
}