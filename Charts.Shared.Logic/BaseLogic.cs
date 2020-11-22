 using System.Threading.Tasks;
using Charts.Shared.Data.Context;
using Charts.Shared.Data.Repos;

namespace Charts.Shared.Logic
{
    public class BaseLogic : IBaseLogic
    {
        private readonly DataContext _context = null;
        public BaseLogic(DataContext context)
        {
            _context = context;
        }


        public BaseRepo<T> Base<T>() where T : BaseEntity
        {
            return new BaseRepo<T>(_context);
        }

    }
}
