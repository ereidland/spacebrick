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

        public UnityEngine.Vector3 ToVector3()
        {
            return new UnityEngine.Vector3(x, y, z);
        }

        public override bool Equals (object obj)
        {
            if (obj is Vector3i)
                return Equals((Vector3i)obj);

            return false;
        }

        public override string ToString () { return string.Format("({0}, {1}, {2})", x, y, z); }

        public void Invert()
        {
            x = -x;
            y = -y;
            z = -z;
        }

        public void Set(int x, int y, int z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }

        public void Set(Vector3i other)
        {
            x = other.x;
            y = other.y;
            z = other.z;
        }

        public void Subtract(Vector3i other)
        {
            x -= other.x;
            y -= other.y;
            z -= other.z;
        }

        public void Add(Vector3i other)
        {
            x += other.x;
            y += other.y;
            z += other.z;
        }

        public void ScaleBy(Vector3i other)
        {
            x *= other.x;
            y *= other.y;
            z *= other.z;
        }

        public void ScaleBy(int other)
        {
            x *= other;
            y *= other;
            z *= other;
        }

        public void DivideBy(Vector3i other)
        {
            if (other.x != 0)
                x /= other.x;
            else
                x = 0;

            if (other.y != 0)
                y /= other.y;
            else
                y = 0;

            if (other.z != 0)
                z /= other.z;
            else
                z = 0;
        }

        public void DivideBy(int other)
        {
            if (other != 0)
            {
                x /= other;
                y /= other;
                z /= other;
            }
            else
                x = y = z = 0;
        }

        public static bool operator == (Vector3i self, Vector3i other) { return self.Equals(other); }
        public static bool operator != (Vector3i self, Vector3i other) { return !self.Equals(other); }

        public static Vector3i operator + (Vector3i self, Vector3i other)
        {
            self.Add(other);
            return self;
        }

        public static Vector3i operator - (Vector3i self, Vector3i other)
        {
            self.Subtract(other);
            return self;
        }

        public static Vector3i operator - (Vector3i self)
        {
            self.Invert();
            return self;
        }

        public static Vector3i operator * (Vector3i self, Vector3i other)
        {
            self.ScaleBy(other);
            return self;
        }

        public static Vector3i operator * (Vector3i self, int other)
        {
            self.ScaleBy(other);
            return self;
        }

        public static Vector3i operator / (Vector3i self, Vector3i other)
        {
            self.DivideBy(other);
            return self;
        }

        public static Vector3i operator / (Vector3i self, int other)
        {
            self.DivideBy(other);
            return self;
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

