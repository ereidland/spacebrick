using System;

namespace Spacetile
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

        private void MoveTree(int source, int dest)
        {
            MoveItem(source, dest);

            int sourceLeft = GetLeft(source);
            int sourceRight = GetRight(source);

            int destLeft = GetLeft(dest);
            int destRight = GetRight(dest);

            if (LeafExists(sourceLeft))
                MoveTree(sourceLeft, destLeft);

            if (LeafExists(sourceRight))
                MoveTree(sourceRight, destRight);
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