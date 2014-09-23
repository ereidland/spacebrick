//
// Vector3i.cs
//
// Author:
//       Evan Reidland <er@evanreidland.com>
//
// Copyright (c) 2014 Evan Reidland
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.
using System;

namespace Spacebrick
{
    public struct Vector3i : IComparable<Vector3i>
    {
        public int x, y, z;

        /// <summary>
        /// Unreliable, but it gets rid of the not implemented warning.
        /// </summary>
        public override int GetHashCode() { return x ^ y ^ z; }

        public int CompareTo(Vector3i other)
        {
            int comparison = x.CompareTo(other.x);

            if (comparison == 0)
                comparison = y.CompareTo(other.y);

            if (comparison == 0)
                comparison = z.CompareTo(other.z);

            return comparison;
        }

        public bool Equals(Vector3i other)
        {
            return x == other.x && y == other.y && z == other.z;
        }

        public override bool Equals (object obj)
        {
            if (obj is Vector3i)
                return Equals((Vector3i)obj);

            return false;
        }

        public Vector3i(int x = 0, int y = 0, int z = 0)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }

        public Vector3i(Vector3i other)
        {
            x = other.x;
            y = other.y;
            z = other.z;
        }
    }
}

