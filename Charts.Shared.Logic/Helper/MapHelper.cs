using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Charts.Shared.Logic.Helper
{
    public static class MapHelper
    {
        public static IEnumerable<TDest> CreateMappedObjects<TSource, TDest>(this IEnumerable<TSource> source, List<Action<TSource, TDest>> mappings = null) where TDest : new()
        {
            List<TDest> dest = new List<TDest>();
            return MapObjects<TSource, TDest>(source, dest, mappings);
        }

        public static TDest CreateMappedObject<TSource, TDest>(this TSource source, List<Action<TSource, TDest>> mappings = null) where TDest : new() where TSource:class
        {
            TDest dest = new TDest();
            return MapObject<TSource, TDest>(source, mappings);
        }

        private static void CopyMatchingProperties<TSource, TDest>(TSource source, TDest dest)
        {
            foreach (var destProp in typeof(TDest).GetProperties(BindingFlags.Public | BindingFlags.Instance).Where(p => p.CanWrite))
            {
                var sourceProp =
                    typeof(TSource).GetProperties(BindingFlags.Public | BindingFlags.Instance).
                        FirstOrDefault(p => p.Name == destProp.Name && p.PropertyType == destProp.PropertyType);

                if (sourceProp != null)
                {
                    destProp.SetValue(dest, sourceProp.GetValue(source, null), null);
                }
            }
        }

        private static List<TDest> MapObjects<TSource, TDest>(IEnumerable<TSource> sources, List<TDest> dests, List<Action<TSource, TDest>> mappings = null) where TDest : new()
        {
            foreach (var source in sources)
            {
                var dest = new TDest();
                CopyMatchingProperties(source, dest);
                if (mappings != null)
                { 
                    foreach (var action in mappings)
                    {
                        action(source, dest);
                    }
                }
                dests.Add(dest);
            }
            return dests;
        }
        private static TDest MapObject<TSource, TDest>(TSource source, List<Action<TSource, TDest>> mappings = null) where TDest : new()
        {
            var dest = new TDest();
            CopyMatchingProperties(source, dest);
            if (mappings != null)
            {
                foreach (var action in mappings)
                {
                    action(source, dest);
                }
            }
           
            return dest;
        }

        public static TDest MapTo<TSource, TDest>(this TSource source, TDest dst, List<Action<TSource, TDest>> mappings = null) where TDest : class
        {
            CopyMatchingProperties(source, dst);
            if (mappings != null)
            {
                foreach (var action in mappings)
                {
                    action(source, dst);
                }
            }
            return dst;
        }
    }
}
