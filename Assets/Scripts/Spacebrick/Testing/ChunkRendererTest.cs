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
    //Test is no longer valid. Needs to be re-written with a full MapRenderer.
    public class ChunkRendererTest : MonoBehaviour
    {
        private BrickMapRenderer _mapRenderer;
        private void Start()
        {
            List<BrickTypeInfo> knownTypes = new List<BrickTypeInfo>();

            _mapRenderer = gameObject.AddComponent<BrickMapRenderer>();

            //TODO: Proper way for loading config.
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
                    Debug.Log("What is " + brickPrefab.name + " doing here without a BrickPrefabConfig Component?");
            }

            var map = new BrickMap(new EventHub());
            _mapRenderer.AssignMap(map);

            const int randomRange = 100;

            var directions = System.Enum.GetValues(typeof(BrickDirection));
            for (int i = 0; i < 5000; i++)
            {
                var type = knownTypes[Random.Range(0, knownTypes.Count)];
                int width = Random.Range(1, 15);
                int height = Random.Range(1, 15);
                int depth = Random.Range(1, 15);
                Vector3i worldPosition = new Vector3i(Random.Range(-randomRange, randomRange), Random.Range(-randomRange, randomRange), Random.Range(-randomRange, randomRange));
                Vector3i localPosition = BrickChunk.GetChunkLocalPosition(worldPosition);

                BrickDirection rotation = (BrickDirection)directions.GetValue(Random.Range(0, directions.Length - 1));

                if (!map.HasAnyOverlappingBricks(worldPosition, new BitRect(0, 0, 0, width, height, depth)))
                    map.AddBrick(worldPosition, new Brick(new BitRect(localPosition.x, localPosition.y, localPosition.z, width, height, depth), new BrickInfo(type.ID, rotation)));
            }
        }


        private void OnDrawGizmos()
        {
            Vector3 center = transform.TransformPoint(Vector3.one*BrickChunk.ChunkSize*0.5f);
            Gizmos.DrawWireCube(center, Vector3.one*BrickChunk.ChunkSize);
        }
    }
}

