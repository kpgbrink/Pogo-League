using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Extensions
{
    public static class ListExtensions
    {
        public static T ChooseRandom<T>(this List<T> list) =>
            list[UnityEngine.Random.Range(0, list.Count - 1)];

        public static int MaxIntOrZero(this List<int> list) {
            try
            {
                return list.Max();
            } catch(InvalidOperationException)
            {
                return 0;
            }
        }
    }
}
