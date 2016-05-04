using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using NLua;

namespace Grammars
{
    internal static class Extensions
    {
        public static void AddRange<T>(this BindingList<T> extended, IEnumerable<T> items)
        {
            foreach (var item in items)
            {
                extended.Add(item);
            }
        }

        public static void AddRange<T>(this ObservableCollection<T> extended, IEnumerable<T> items)
        {
            foreach (var item in items)
            {
                extended.Add(item);
            }
        }

        public static Dictionary<K, V> ToDictionary<K, V>(this LuaTable extended)
        {
            var result = new Dictionary<K, V>();

            for (var i = 0; i <= extended.Keys.Count - 1; i++)
            {
                result.Add(extended.Keys.Cast<K>().ElementAt(i), extended.Values.Cast<V>().ElementAt(i));
            }

            return result;
        }

        public static List<T> ToList<T>(this LuaTable extended)
        {
            var result = new List<T>();

            for (var i = 0; i <= extended.Values.Count - 1; i++)
            {
                result.Add(extended.Values.Cast<T>().ElementAt(i));
            }

            return result;
        }
    }
}