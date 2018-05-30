using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Spark.Infra.Types
{
    public static class DictionaryExtensions
    {
        public static Optional<TValue> TryGetAValue<TKey, TValue>(this Dictionary<TKey, TValue> dictionary, TKey key)
        {
            if (dictionary.ContainsKey(key))
                return Optional<TValue>.Some(dictionary[key]);
            else
                return Optional<TValue>.None();
        }
    }
}
