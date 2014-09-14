//
// KeyValueTree.cs
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
using System.Collections.Generic;

namespace Spacebrick
{
    public class KeyValueTree<K, V> where K : IComparable
    {
        public struct Leaf
        {
            public K Key;
            public V Value;

            public void Clear()
            {
                Key = default(K);
                Value = default(V);
            }
			
            public Leaf(K key, V value)
            {
                Key = key;
                Value = value;
            }
        }

        private struct FromTo
        {
            public int From;
            public int To;

            public FromTo(int fromIndex, int toIndex)
            {
                From = fromIndex;
                To = toIndex;
            }
        }

        private Leaf[] _heap;

        public System.Collections.Generic.IEnumerable<Leaf> RawLeaves { get { return _heap; } }

        public bool GetExistingLeaf(K key, out V value)
        {
            if (_heap != null)
            {
                int curIndex = 1;
                while (curIndex < _heap.Length)
                {
                    Leaf currentLeaf = _heap[curIndex];
                    int compare = key.CompareTo(currentLeaf.Key);

                    if (compare > 0)
                    {
                        //If we are larger, then look left.
                        curIndex *= 2;
                    }
                    else if (compare < 0)
                    {
                        //If we are smaller, then look right.
                        curIndex = curIndex*2 + 1;
                    }
                    else
                    {
                        //If we are equal, then we found it.
                        value = currentLeaf.Value;
                        return true;
                    }
                }
            }

			value = default(V);
            return false;
        }

        private bool EnsureCapacity(int size)
        {
            if (_heap == null || _heap.Length < size)
            {
                Leaf[] newHeap = new Leaf[size];

                if (_heap != null)
                    _heap.CopyTo(newHeap, 0);

                _heap = newHeap;
                return true;
            }
            return false;
        }

        private int FindIndex(K key, bool create)
        {
            int curIndex = 1;

            while (create || (_heap != null && curIndex < _heap.Length))
            {
                //If we had to reallocate to accomidate this index, then we found our position.
                if (create && EnsureCapacity(curIndex + 1))
                {
                    return curIndex;
                }
                else 
                {
                    //currentKey = key at this index.
                    K currentKey = _heap[curIndex].Key;

                    //If there's nothing at our current index, then use it.
                    if (currentKey.Equals(default(K)))
                        return curIndex;

                    //Compare the desired key to our current key.
                    int compare = key.CompareTo(currentKey);

                    if (compare > 0)
                    {
                        //If we are larger, go left.
                        curIndex = GetLeft(curIndex);
                    }
                    else if (compare < 0)
                    {
                        //If we are smaller, go right.
                        curIndex = GetRight(curIndex);
                    }
                    else
                        return curIndex;
                }
            }

            return -1;
        }

        public void Set(K key, V value)
        {
            int curIndex = FindIndex(key, true);

            _heap[curIndex] = new Leaf(key, value);
        }

        private bool LeafExists(int index)
        {
            return index > 0 && index < _heap.Length && !_heap[index].Key.Equals(default(K));
        }

        private int GetLeft(int index) { return index*2; }
        private int GetRight(int index) { return index*2 + 1; }
        private int GetParent(int index) { return index/2; }

        private bool IsLeft(int index) { return index % 2 == 0; }

        private void MoveItem(int source, int dest)
        {
            _heap[dest] = _heap[source];
            _heap[source].Clear();
        }

        private void MoveTree(int initialSource, int initialDest)
        {
            Queue<FromTo> openQueue = new Queue<FromTo>();

            openQueue.Enqueue(new FromTo(initialSource, initialDest));

            while(openQueue.Count > 0)
            {
                var fromTo = openQueue.Dequeue();
                MoveItem(fromTo.From, fromTo.To);

                int sourceLeft = GetLeft(fromTo.From);
                int sourceRight = GetRight(fromTo.From);

                int destLeft = GetLeft(fromTo.To);
                int destRight = GetRight(fromTo.To);

                if (LeafExists(sourceLeft))
                    openQueue.Enqueue(new FromTo(sourceLeft, destLeft));

                if (LeafExists(sourceRight))
                    openQueue.Enqueue(new FromTo(sourceRight, destRight));
            }
        }

        public void Delete(K key)
        {
            int currentIndex = FindIndex(key, false);
            //We only allow indices 1 or higher.
            if (currentIndex > 0)
            {
                _heap[currentIndex].Clear();

                int left = GetLeft(currentIndex);

                //If we have no left item.
                if (!LeafExists(left))
                {
                    int right = GetRight(currentIndex);
                    //If we have a right item.
                    if (LeafExists(right))
                        MoveTree(right, currentIndex);
                }
                else //We have a left item.
                {
                    int right = GetRight(currentIndex);

                    //If we don't have a right item.
                    if (!LeafExists(right))
                    {
                        MoveTree(left, currentIndex);
                    }
                    else //We have both a left and right item.
                    {
                        //Navigate to far right index of current left node.
                        int farRightIndex = GetLeft(currentIndex);
                        int nextIndex = GetRight(farRightIndex);

                        while(LeafExists(nextIndex))
                        {
                            farRightIndex = nextIndex;
                            nextIndex = GetRight(farRightIndex);
                        }
                            

                        MoveItem(farRightIndex, currentIndex);

                        currentIndex = farRightIndex;
                        left = GetLeft(currentIndex);
                        //If far right has a left branch.
                        if (LeafExists(left))
                        {
                            MoveTree(left, currentIndex);
                        }
                    }
                }
            }
        }

        public void Clear()
        {
            _heap = null;
        }
    }
}