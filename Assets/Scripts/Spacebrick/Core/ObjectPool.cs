﻿//
// ObjectPool.cs
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
    public class ObjectPool<T>
    {
        private Func<T> _create;
        private Action<T> _dispose;

        private Queue<T> _pool = new Queue<T>();

        public IEnumerable<T> FlushPool
        {
            get
            {
                while (_pool.Count > 0)
                    yield return _pool.Dequeue();
            }
        }

        public T Pop()
        {
            if (_pool.Count > 0)
                return _pool.Dequeue();

            return _create();
        }

        public void Push(T item)
        {
            _dispose(item);
            _pool.Enqueue(item);
        }

        private T DefaultCreate() { return Activator.CreateInstance<T>(); }
        private void DefaultDispose(T item) {}

        public ObjectPool(Func<T> createFunction, Action<T> disposeFunction)
        {
            _create = createFunction;
            _dispose = disposeFunction;
        }

        public ObjectPool()
        {
            //Default initialization.
            _create = DefaultCreate;
            _dispose = DefaultDispose;
        }
    }
}

