using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Extensions
{
    public static class DictionaryExtensions
    {
        public static int? MaxValueIntKey(this Dictionary<int, int> dictionary)
        {
            if (dictionary.Values.Count == 0)
            {
                return null;
            }
            return dictionary.Aggregate((x, y) => x.Value > y.Value ? x : y).Value;
        }
    }
}
