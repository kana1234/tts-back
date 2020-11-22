using System.Linq;
using System.Threading.Tasks;
using Charts.Shared.Data.Context;
using Charts.Shared.Data.Context.Dictionary;
using Charts.Shared.Data.Extensions;
using Charts.Shared.Data.Repos.Dictionary;
using Microsoft.EntityFrameworkCore;

namespace Charts.Shared.Logic.Dictionary
{
    public class DictionaryLogic : IDictionaryLogic
    {
        private readonly DataContext _context = null;
        public DictionaryLogic(DataContext context)
        {
            _context = context;
        }

        public Task<T> Add<T>(T model) where T : BaseDictionary
        {
            var repo = new DictionaryRepo<T>(_context);
            return repo.Add(model);
        }

        public DictionaryRepo<T> DictionaryRepo<T>() where T : BaseDictionary
        {
            return new DictionaryRepo<T>(_context);
        }

        public object DictionaryRepoGetDtoList<T>() where T : BaseDictionary
        {
            return new DictionaryRepo<T>(_context).GetQueryable().AsNoTracking().Select(x => x.ToDto()); ;
        }

        public Task Update<T>(T model) where T : BaseDictionary
        {
            var repo = new DictionaryRepo<T>(_context);
            return repo.Update(model);
        }
    }
}
