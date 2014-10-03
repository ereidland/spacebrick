//
// MeshPool.cs
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
using UnityEngine;

namespace Spacebrick
{
    public class StaticMeshPool : MonoBehaviour
    {
        private static StaticMeshPool _instance;

        private static void EnsureInstance()
        {
            if (_instance == null)
                _instance = new GameObject("_StaticMeshPool").AddComponent<StaticMeshPool>();
        }

        private ObjectPool<Mesh> _pool = new ObjectPool<Mesh>();

        public static Mesh GetMesh()
        {
            EnsureInstance();
            return _instance._pool.Pop();
        }

        public static void ReleaseMesh(Mesh mesh)
        {
            EnsureInstance();
            _instance._pool.Push(mesh);
        }

        private void OnDestroy()
        {
            foreach (var mesh in _pool.FlushPool)
            {
                if (mesh != null)
                    Destroy(mesh);
            }
        }
    }
}

