using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Text.RegularExpressions;
using System.IO;

namespace AutoLevelMenu
{
    public static class Utils
    {
        public static string[] GetEnumNames<T>()
        {
            var array = Enum.GetNames(typeof(T));
            return array;
        }

        public static string RemoveNumericStart(string str)
        {
            return Regex.Replace(str, @"^[\d\.]*\s*", "");
        }

        public static string GetSceneName(string scenePath, bool replaceNumber = false)
        {
            var name = Path.GetFileNameWithoutExtension(scenePath);
            if (replaceNumber)
            {
                name = RemoveNumericStart(name);
            }
            return name;
        }

        // From Stackoverflow: https://stackoverflow.com/a/16193323/2948122
        // Question: https://stackoverflow.com/questions/16192906/net-dictionary-get-or-create-new
        // Question by Rok Strniša: https://stackoverflow.com/users/974531/rok-strni%C5%A1a
        // Answer by Adam Houldsworth: https://stackoverflow.com/users/358221/adam-houldsworth
        public static TValue GetOrCreate<TKey, TValue>(this IDictionary<TKey, TValue> dict, TKey key)
        where TValue : new()
        {
            TValue val;
            if (!dict.TryGetValue(key, out val))
            {
                val = new TValue();
                dict.Add(key, val);
            }
            return val;
        }

        // TODO make this check if valid paths
        public static bool PathStartsWithPath(string path, string startsWithPath)
        {
            if (path == "")
            {
                return false;
            }
            return Path.GetFullPath(path).StartsWith(Path.GetFullPath(startsWithPath));
        }

        public static string ToForwardSlash(string path)
        {
            return path.Replace("\\", "/");
        }

        // https://stackoverflow.com/a/20175/2948122
        public static string AddOrdinal(int num)
        {
            if (num <= 0) return num.ToString();

            switch (num % 100)
            {
                case 11:
                case 12:
                case 13:
                    return num + "th";
            }

            switch (num % 10)
            {
                case 1:
                    return num + "st";
                case 2:
                    return num + "nd";
                case 3:
                    return num + "rd";
                default:
                    return num + "th";
            }
        }

        // https://forum.unity.com/threads/get-the-layernumber-from-a-layermask.114553/
        public static int MaskToLayer(LayerMask mask)
        {
            var bitmask = mask.value;

            UnityEngine.Assertions.Assert.IsFalse((bitmask & (bitmask - 1)) != 0,
            "MaskToLayer() was passed an invalid mask containing multiple layers.");

            var result = bitmask > 0 ? 0 : 31;
            while (bitmask > 1)
            {
                bitmask >>= 1;
                result++;
            }
            return result;
        }

        public static bool CheckPastPlane(Transform plane, Transform transform)
        {
            var resultingUpVector = plane.rotation * Vector3.up;
            var dot = Vector3.Dot(resultingUpVector, plane.position - transform.position);
            return dot < 0;
        }
    }
}
