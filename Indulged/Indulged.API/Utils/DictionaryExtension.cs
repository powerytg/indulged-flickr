using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Indulged.API.Utils
{
    public static class DictionaryExtension
    {
        public static Dictionary<TKey, TValue> Merge<TKey, TValue>(this IEnumerable<Dictionary<TKey, TValue>> enumerable)
        {
            return enumerable.SelectMany(x => x).ToDictionary(x => x.Key, y => y.Value);
        }
    }
}
