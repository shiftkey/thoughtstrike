using System.Collections.Generic;

namespace Ideastrike.Models.Repositories
{
    public interface IImageRepository
    {
        IEnumerable<Image> GetAll();
        Image Get(int id);

        void Add(Image image);
        void Delete(int id);
        
    }
}