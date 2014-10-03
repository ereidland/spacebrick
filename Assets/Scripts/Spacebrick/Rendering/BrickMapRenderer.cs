//
// BrickMapRenderer.cs
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
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Spacebrick
{
    public class BrickMapRenderer : MonoBehaviour
    {
        private KeyValueTree<Vector3i, BrickChunkRenderer> _chunkRenderers = new KeyValueTree<Vector3i, BrickChunkRenderer>();
        private Queue<BrickChunkRenderer> _chunksToBuild = new Queue<BrickChunkRenderer>();

        public BrickMap Map { get; private set; }

        public int MaxBuildsPerFrame = 100;

        /// <summary>
        /// Assigns our map and listens for events from it.
        /// Only works if current Map is null.
        /// </summary>
        /// <param name="map">Map.</param>
        public void AssignMap(BrickMap map)
        {
            if (Map == null)
            {
                Map = map;

                if (Map != null)
                {
                    Map.Events.AddReceiver<ChunkCreatedEvent>(OnChunkCreated);
                    Map.Events.AddReceiver<BrickModifiedEvent>(OnBrickModified);
                }
            }
        }

        private void OnChunkCreated(ChunkCreatedEvent e)
        {
            AddChunk(e.NewChunk);
        }

        private void OnBrickModified(BrickModifiedEvent e)
        {
            BrickChunkRenderer chunkRenderer;
            if (_chunkRenderers.TryGet(e.Chunk.ChunkPosition, out chunkRenderer))
            {
                AddToBuildQueue(chunkRenderer);
            }
            else
            {
                Debug.LogError("No chunk renderer at " + e.Chunk.ChunkPosition.ToString() + "... Auto creating.");
                AddChunk(e.Chunk);
            }
        }

        private void AddToBuildQueue(BrickChunkRenderer chunkRenderer)
        {
            if (chunkRenderer != null && !_chunksToBuild.Contains(chunkRenderer))
                _chunksToBuild.Enqueue(chunkRenderer);
        }

        private void ProcessQueueItem()
        {
            if (_chunksToBuild.Count > 0)
            {
                var chunkToBuild = _chunksToBuild.Dequeue();
                if (chunkToBuild == null)
                    ProcessQueueItem();
                else
                    chunkToBuild.BuildMeshes();
            }
        }

        private BrickChunkRenderer AddChunk(BrickChunk chunk)
        {
            var chunkRendererObject = new GameObject("Chunk " + chunk.ChunkPosition.ToString());
            var chunkRenderer = chunkRendererObject.AddComponent<BrickChunkRenderer>();

            chunkRenderer.AssignedChunk = chunk;

            chunkRenderer.transform.parent = transform;
            chunkRenderer.transform.localPosition = chunk.WorldPosition.ToVector3();
            chunkRenderer.transform.localScale = Vector3.one;
            chunkRenderer.transform.localRotation = Quaternion.identity;

            _chunkRenderers.Set(chunk.ChunkPosition, chunkRenderer);

            AddToBuildQueue(chunkRenderer);
            return chunkRenderer;
        }

        private void Update()
        {
            for (int i = 0; i < MaxBuildsPerFrame; i++)
                ProcessQueueItem();
        }
    }
}

