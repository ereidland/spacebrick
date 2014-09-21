//
// ChunkRendererTest.cs
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
using System.Collections.Generic;
using UnityEngine;

namespace Spacebrick
{
    public class ChunkRendererTest : MonoBehaviour
    {
        private BrickChunkRenderer _chunkRenderer;
        private void Start()
        {
            List<BrickTypeInfo> knownTypes = new List<BrickTypeInfo>();

            //Not intended use case. Trying to just figure out if everything works.
            _chunkRenderer = gameObject.AddComponent<BrickChunkRenderer>();

            var brickPrefabs = Resources.LoadAll<GameObject>("Spacebricks");
            foreach (var brickPrefab in brickPrefabs)
            {
                var brickPrefabConfig = brickPrefab.GetComponent<BrickPrefabConfig>();
                if (brickPrefabConfig != null)
                {
                    brickPrefabConfig.Register();
                    knownTypes.Add(brickPrefabConfig.TypeInfo);
                }
                else
                    Debug.Log("What is " + brickPrefab.name + " doing here without a BrickPrefabConfigComponent?");
            }

            var chunk = new BrickChunk(new Vector3i());
            var directions = System.Enum.GetValues(typeof(BrickDirection));
            for (int i = 0; i < 10; i++)
            {
                var type = knownTypes[Random.Range(0, knownTypes.Count)];
                int width = Random.Range(1, 4);
                int height = Random.Range(1, 4);
                int depth = Random.Range(1, 4);
                int x = Random.Range(0, BrickChunk.ChunkSize);
                int y = Random.Range(0, BrickChunk.ChunkSize);
                int z = Random.Range(0, BrickChunk.ChunkSize);

                BrickDirection rotation = (BrickDirection)directions.GetValue(Random.Range(0, directions.Length - 1));

                chunk.SetBrick(new Brick(new BitRect(x, y, z, width, height, depth), new BrickInfo(type.ID, rotation)));
            }

            _chunkRenderer.BuildMeshes(chunk);
        }
    }
}

