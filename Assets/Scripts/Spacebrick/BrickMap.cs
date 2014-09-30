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

        public BrickChunk GetChunk(Vector3i position)
        {
            if (_lastChunk != null && _lastChunk.ChunkPosition == position)
                return _lastChunk;
                
            _chunks.TryGet(position, out _lastChunk);
            return _lastChunk;
        }

        public BrickChunk GetOrCreateChunk(Vector3i position)
        {
            var chunk = GetChunk(position);
            if (chunk == null)
            {
                chunk = _lastChunk = new BrickChunk(this, position);
                _chunks.Set(position, chunk);

                _chunkCreatedList.Execute(new ChunkCreatedEvent(chunk));
                return chunk;
            }

            return chunk;
        }

        public BrickMap(EventHub events)
        {
            Events = events;
            _chunkCreatedList = Events.GetList(typeof(ChunkCreatedEvent));
        }
    }
}

