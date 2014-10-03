//
// BrickMap.cs
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
    public class ChunkCreatedEvent
    {
        public BrickChunk NewChunk { get; private set; }
        public ChunkCreatedEvent(BrickChunk newChunk)
        {
            NewChunk = newChunk;
        }
    }

    public class BrickMap
    {
        private KeyValueTree<Vector3i, BrickChunk> _chunks = new KeyValueTree<Vector3i, BrickChunk>();
        private BrickChunk _lastChunk;

        public EventHub Events { get; private set; }

        private EventCallbackList _chunkCreatedList;

        public BrickChunk GetChunk(Vector3i chunkPosition)
        {
            if (_lastChunk != null && _lastChunk.ChunkPosition == chunkPosition)
                return _lastChunk;
                
            _chunks.TryGet(chunkPosition, out _lastChunk);
            return _lastChunk;
        }

        public BrickChunk GetOrCreateChunk(Vector3i chunkPosition)
        {
            var chunk = GetChunk(chunkPosition);
            if (chunk == null)
            {
                chunk = _lastChunk = new BrickChunk(this, chunkPosition);
                _chunks.Set(chunkPosition, chunk);

                _chunkCreatedList.Execute(new ChunkCreatedEvent(chunk));
                return chunk;
            }

            return chunk;
        }

        private void DoForRelevantChunks(Func<BrickChunk, bool> callback, Vector3i chunkPosition)
        {
            for (int ix = 0; ix >= -1; ix--)
            {
                for (int iy = 0; iy >= -1; iy--)
                {
                    for (int iz = 0; iz >= -1; iz--)
                    {
                        var chunk = GetChunk(new Vector3i(chunkPosition.x + ix, chunkPosition.y + iy, chunkPosition.z + iz));
                        if (chunk != null && callback(chunk))
                            break;
                    }
                }
            }
        }

        public Brick GetOverlappingBrick(Vector3i worldPosition)
        {
            var brick = new Brick();
            DoForRelevantChunks((chunk) =>
            {
                Vector3i localPosition = BrickChunk.GetChunkLocalPosition(worldPosition);
                brick = chunk.GetBrick(localPosition.x, localPosition.y, localPosition.z);
                return !brick.IsEmpty;
            }, BrickChunk.GetChunkPosition(worldPosition));

            return brick;
        }

        public bool HasAnyOverlappingBricks(Vector3i worldPosition, BitRect rect)
        {
            Vector3i localPosition = BrickChunk.GetChunkLocalPosition(worldPosition);
            Vector3i chunkPosition = BrickChunk.GetChunkPosition(worldPosition);

            rect.X = (byte)localPosition.x;
            rect.Y = (byte)localPosition.y;
            rect.Z = (byte)localPosition.z;

            bool anyOverlapping = false;

            DoForRelevantChunks((chunk) =>
            {
                BitRect localRect = chunk.LocalizeRect(chunkPosition, rect);

                if (localRect.Width > 0)
                    anyOverlapping = chunk.HasAnyOverlappingBricks(localRect);
                return anyOverlapping;
            }, BrickChunk.GetChunkPosition(worldPosition));
            
            return anyOverlapping;
        }

        /// <summary>
        /// Adds specified brick to the chunk found or created at the given world position.
        /// Does not do any checks to determine if this is a valid operation (whether it overlaps other bricks).
        /// </summary>
        public void AddBrick(Vector3i worldPosition, Brick brick)
        {
            Vector3i chunkPosition = BrickChunk.GetChunkPosition(worldPosition);
            var chunk = GetOrCreateChunk(chunkPosition);
            chunk.AddBrick(brick);
        }

        /// <summary>
        /// Gets the overlapping bricks for the specified BitRect at the given world position.
        /// World position is used to calculate the rect positional coordinates within the chunk.
        /// </summary>
        public List<Brick> GetOverlappingBricks(Vector3i worldPosition, BitRect rect)
        {
            List<Brick> overlappingBricks = new List<Brick>();

            Vector3i localPosition = BrickChunk.GetChunkLocalPosition(worldPosition);
            Vector3i chunkPosition = BrickChunk.GetChunkPosition(worldPosition);

            rect.X = (byte)localPosition.x;
            rect.Y = (byte)localPosition.y;
            rect.Z = (byte)localPosition.z;

            DoForRelevantChunks((chunk) =>
            {
                BitRect localRect = chunk.LocalizeRect(chunkPosition, rect);
                
                if (localRect.Width > 0)
                    chunk.GetOverlappingBricks(overlappingBricks, rect);
                
                return false;
            }, BrickChunk.GetChunkPosition(worldPosition));

            return overlappingBricks;
        }

        public BrickMap(EventHub events)
        {
            Events = events;
            _chunkCreatedList = Events.GetList(typeof(ChunkCreatedEvent));
        }
    }
}

