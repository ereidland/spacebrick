//
// BrickChunk.cs
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
    public enum BrickModification
    {
        Created,
        Removed,
    }

    public class BrickModifiedEvent
    {
        public BrickChunk Chunk { get; private set; }
        public Brick ModifiedBrick { get; private set; }
        public BrickModification Modification { get; private set; }

        public BrickModifiedEvent(BrickChunk chunk, Brick brick, BrickModification modification)
        {
            Chunk = chunk;
            ModifiedBrick = brick;
            Modification = modification;
        }
    }

    public class BrickChunk
    {
        public const int ChunkSize = 16;
        public const int ChunkSizeMask = 15;
        public const int ChunkSizeShift = 4;

        public static Vector3i GetWorldPosition(Vector3i chunkPosition)
        {
            return new Vector3i(chunkPosition.x << ChunkSizeShift, chunkPosition.y << ChunkSizeShift, chunkPosition.z << ChunkSizeShift);
        }

        public static Vector3i GetChunkPosition(Vector3i worldPosition)
        {
            return new Vector3i(worldPosition.x >> ChunkSizeShift, worldPosition.y >> ChunkSizeShift, worldPosition.z >> ChunkSizeShift);
        }

        public static void GetChunkLocalPosition(ref int x, ref int y, ref int z)
        {
            x &= ChunkSizeMask;
            y &= ChunkSizeMask;
            z &= ChunkSizeMask;
        }

        public static Vector3i GetChunkLocalPosition(Vector3i worldPosition)
        {
            return new Vector3i(worldPosition.x & ChunkSizeMask, worldPosition.y & ChunkSizeMask, worldPosition.z & ChunkSizeMask);
        }

        public Vector3i ChunkPosition { get; private set; }
        public Vector3i WorldPosition { get { return GetWorldPosition(ChunkPosition); } }
        public BrickMap Map { get; private set; }

        private EventCallbackList _brickModifiedList;

        private void NotifyBrickCreated(Brick brick)
        {
            _brickModifiedList.Execute(new BrickModifiedEvent(this, brick, BrickModification.Created));
        }

        private void NotifyBrickRemoved(Brick brick)
        {
            _brickModifiedList.Execute(new BrickModifiedEvent(this, brick, BrickModification.Removed));
        }

        private bool ContainsLocally(int x, int y, int z)
        {
            return x >= 0 && y >= 0 && z >= 0 && x < ChunkSize && y < ChunkSize && z < ChunkSize;
        }

        public BitRect LocalizeRect(Vector3i otherChunkPosition, BitRect rect)
        {
            Vector3i chunkWorldPosition = WorldPosition;
            Vector3i otherChunkWorldPosition = GetWorldPosition(otherChunkPosition);
            Vector3i offset = otherChunkWorldPosition - chunkWorldPosition;

            int x = rect.X;
            int y = rect.Y;
            int z = rect.Z;
            int width = rect.Width;
            int height = rect.Height;
            int depth = rect.Depth;

            x += offset.x;
            y += offset.y; 
            z += offset.z;

            if (x < 0)
                width += x;

            if (y < 0)
                height += y;

            if (z < 0)
                depth += z;

            if (width > 0 && height > 0 && depth > 0)
            {
                x = Math.Max(x, 0);
                y = Math.Max(y, 0);
                z = Math.Max(z, 0);

                return new BitRect(x, y, z, width, height, depth);
            }

            return new BitRect();
        }

        private List<Brick> _bricks = new List<Brick>();

        public IEnumerable<Brick> Bricks
        {
            get
            {
                for (int i = 0; i < _bricks.Count; i++)
                    yield return _bricks[i];
            }
        }

        public int BrickCount { get { return _bricks.Count; } }

        public void GetOverlappingBricks(List<Brick> overlappingBricks, BitRect rect)
        {
            for(int i = 0; i < _bricks.Count; i++)
            {
                var brick = _bricks[i];
                if (brick.Rect.Intersects(rect))
                    overlappingBricks.Add(brick);
            }
        }

        public List<Brick> GetOverlappingBricks(BitRect rect)
        {
            List<Brick> overlappingBricks = new List<Brick>();
            GetOverlappingBricks(overlappingBricks, rect);
            return overlappingBricks;
        }

        public Brick GetBrick(int x, int y, int z)
        {
            for (int i = 0; i < _bricks.Count; i++)
            {
                var brick = _bricks[i];
                if (brick.Rect.Contains(x, y, z))
                    return brick;
            }

            return new Brick();
        }

        public bool HasAnyOverlappingBricks(int x, int y, int z, int width, int height, int depth)
        {
            for (int i = 0; i < _bricks.Count; i++)
                if (_bricks[i].Rect.Intersects(x, y, z, width, height, depth))
                    return true;

            return false;
        }

        public bool HasAnyOverlappingBricks(BitRect rect)
        {
            for(int i = 0; i < _bricks.Count; i++)
                if (_bricks[i].Rect.Intersects(rect))
                    return true;

            return false;
        }

        private int FindSlotForBrick()
        {
            //Check for empty bricks in list.
            for (int i = 0; i < _bricks.Count; i++)
            {
                if (_bricks[i].IsEmpty)
                    return i;
            }

            //No empty bricks? Add one to the end.
            _bricks.Add(new Brick());
            return _bricks.Count - 1;
        }

        public void AddBrick(Brick brick)
        {
            _bricks.Add(brick);
            NotifyBrickCreated(brick);
        }

        public void RemoveBrickAtIndex(int index)
        {
            if (index >= 0 && index < _bricks.Count)
            {
                var existingBrick = _bricks[index];
                _bricks.RemoveAt(index);

                if (!existingBrick.IsEmpty)
                    NotifyBrickRemoved(existingBrick);
            }
        }

        public Brick GetBrickAtIndex(int index)
        {
            if (index >= 0 && index < _bricks.Count)
                return _bricks[index];

            return new Brick();
        }

        public void SetBrickAtIndex(int index, Brick brick)
        {
            if (index >= 0 && index < _bricks.Count)
            {
                var existingBrick = _bricks[index];
                _bricks[index] = brick;

                if (!existingBrick.IsEmpty)
                    NotifyBrickRemoved(existingBrick);

                NotifyBrickCreated(brick);
            }
        }

        public BrickChunk(BrickMap map, Vector3i position)
        {
            Map = map;
            ChunkPosition = position;

            _brickModifiedList = map.Events.GetList(typeof(BrickModifiedEvent));
        }
    }
}

