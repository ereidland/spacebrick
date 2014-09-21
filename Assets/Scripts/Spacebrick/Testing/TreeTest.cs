//
// TreeTest.cs
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

using UnityEngine;
using Spacebrick;

namespace Spacebrick
{
    public class TreeTest : MonoBehaviour
    {
        public int NumIterations = 2000;
        public int NumDeletions = 1000;

        public int MinRange = 1;
        public int MaxRange = 1000;

        private void Start()
        {
            KeyValueTree<int, string> tree = new KeyValueTree<int, string>();

            var watch = new System.Diagnostics.Stopwatch();

            watch.Start();
            for (int i = 0; i < NumIterations; i++)
            {
                int key = Random.Range(MinRange, MaxRange);
                string value = "Hi.";

                if (NumDeletions > 0 && Random.value > 0.5f)
                {
                    NumDeletions--;
                    tree.Delete(key);
                }
                else
                {
                    tree.Set(key, value);
                }
            }

            watch.Stop();

            foreach (var leaf in tree.RawLeaves)
            {
                if (leaf.Key != 0)
                    Debug.Log(leaf.Key + " - " + leaf.Value);
            }

            Debug.Log(string.Format("{0} iterations took {1} milliseconds, at {2} iterations/millisecond.", NumIterations, watch.ElapsedMilliseconds, NumIterations/(float)watch.ElapsedMilliseconds));
        }
    }
}