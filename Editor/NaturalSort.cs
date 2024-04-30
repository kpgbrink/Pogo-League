// Copyright 2019 Nathan Phillip Brink
//
// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to
// the following conditions:
//
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT.
// IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY
// CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT,
// TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE
// SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
// https://gist.github.com/binki/66872fa978d40dc51afa7ce239d0e7a2

using System;
using System.Linq;
using System.Text.RegularExpressions;

namespace AutoLevelMenu.EditorNS
{
    public class NaturalSort
    {
        public static SegmentedComparable BuildComparable(string value)
        {
            var captures = Regex.Match(value, @"(?:(\d+|\w+|\W))*").Groups[1].Captures;
            return new SegmentedComparable(Enumerable.Range(0, captures.Count).Select(i => new SegmentComparable(captures[i].Value)).ToArray());
        }

        public class SegmentedComparable : IComparable<SegmentedComparable>
        {
            readonly SegmentComparable[] segments;
            public SegmentedComparable(SegmentComparable[] segments)
            {
                this.segments = segments ?? throw new ArgumentNullException(nameof(segments));
            }
            public int CompareTo(SegmentedComparable other)
            {
                for (var i = 0; i < segments.Length; i++)
                {
                    if (other.segments.Length <= i)
                    {
                        return -1;
                    }
                    var result = segments[i].CompareTo(other.segments[i]);
                    if (result != 0)
                    {
                        return result;
                    }
                }
                if (other.segments.Length > segments.Length)
                {
                    return 1;
                }
                return 0;
            }
        }

        public class SegmentComparable : IComparable<SegmentComparable>
        {
            readonly bool isNumeric;
            readonly string value;
            public SegmentComparable(string value)
            {
                this.value = value;
                isNumeric = value.All(char.IsNumber);
            }
            public int CompareTo(SegmentComparable other)
            {
                var myCompareValue = value;
                var otherCompareValue = other.value;
                if (isNumeric && other.isNumeric)
                {
                    // Pad with zeros.
                    if (myCompareValue.Length < otherCompareValue.Length)
                    {
                        myCompareValue = string.Concat(Enumerable.Repeat("0", otherCompareValue.Length - myCompareValue.Length)) + myCompareValue;
                    }
                    else
                    {
                        otherCompareValue = string.Concat(Enumerable.Repeat("0", myCompareValue.Length - otherCompareValue.Length)) + otherCompareValue;
                    }
                }
                return myCompareValue.CompareTo(otherCompareValue);
            }
        }
    }
}
