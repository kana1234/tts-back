using System;
using System.Collections.Generic;

namespace Charts.Shared.Data.Extensions
{
    public static class EnumExtHelper
    {
        public static Dictionary<int, string> GetEnumDictionary<TEnum>()
        {
            if (typeof(TEnum).IsEnum)
            {
                var _ = new Dictionary<int, string>();
                foreach (int val in Enum.GetValues(typeof(TEnum)))
                {
                    _.Add(val, Enum.GetName(typeof(TEnum), val));
                }
                return _;
            }
            throw new Exception($"not Enum argument");
        }
    }
}
