using Charts.Shared.Data.Context.Dictionary;
using Charts.Shared.Data.Models.Dictionary;

namespace Charts.Shared.Data.Extensions
{
    public static class DictionaryExt
    {
        public static BaseDictionaryDto ToDto(this BaseDictionary x)
        {
            if (x == null)
                return null;

            return new BaseDictionaryDto
            {
                Id = x.Id,
                NameRu = x.NameRu,
                NameKz = x.NameKz
            };
        }

        public static BaseDictionary ToEntity(this BaseDictionaryDto x)
        {
            if (x == null)
                return null;
            var _ = new BaseDictionary
            {
                NameRu = x.NameRu,
                NameKz = x.NameKz
            };
            if (x.Id != null)
                _.Id = x.Id.Value;
            return _;
        }
    }
}
