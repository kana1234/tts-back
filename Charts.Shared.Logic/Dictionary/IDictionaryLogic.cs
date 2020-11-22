using System.Threading.Tasks;
using Charts.Shared.Data.Context.Dictionary;
using Charts.Shared.Data.Repos.Dictionary;

namespace Charts.Shared.Logic.Dictionary
{
    public interface IDictionaryLogic
    {
        DictionaryRepo<T> DictionaryRepo<T>() where T : BaseDictionary;
        Task<T> Add<T>(T model) where T : BaseDictionary;
        Task Update<T>(T model) where T : BaseDictionary;
        object DictionaryRepoGetDtoList<T>() where T : BaseDictionary;
    }
}
