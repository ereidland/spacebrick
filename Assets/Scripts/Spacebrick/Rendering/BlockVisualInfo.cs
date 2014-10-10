//
// BrickVisualInfo.cs
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
#if UNITY
using UnityEngine;
#endif

namespace Spacebrick
{
    public static class BrickVisualHelpers
    {
        private static Quaternion Forward = Quaternion.identity;
        private static Quaternion Right = Quaternion.AngleAxis(90, Vector3.up);
        private static Quaternion Back = Quaternion.AngleAxis(180, Vector3.up);
        private static Quaternion Left = Quaternion.AngleAxis(-90, Vector3.up);
        private static Quaternion Down = Quaternion.AngleAxis(90, Vector3.right);
        private static Quaternion Up = Quaternion.AngleAxis(-90, Vector3.right);

        public static Quaternion ToRotation(this BlockDirection direciton)
        {
            switch (direciton)
            {
                case BlockDirection.Right:
                    return Right;
                case BlockDirection.Back:
                    return Back;
                case BlockDirection.Left:
                    return Left;
                case BlockDirection.Down:
                    return Down;
                case BlockDirection.Up:
                    return Up;
                case BlockDirection.Forward:
                default:
                    return Forward;
            }
        }
    }
    public class BlockVisualInfo
    {
        private static NameIDRegistry<BlockVisualInfo> _brickVisualRegistry = new NameIDRegistry<BlockVisualInfo>();

        public BlockTypeInfo BrickType { get; private set; }
        public IBrickMeshBuilder Builder { get; private set; }

        public static BlockVisualInfo GetVisualInfo(string name) { return _brickVisualRegistry.GetItem(name); }
        public static BlockVisualInfo GetVisualInfo(int id) { return _brickVisualRegistry.GetItem(id); }

        public static bool RegisterBuilder(string name, IBrickMeshBuilder builder) { return RegisterBuilder(BlockTypeInfo.GetTypeInfo(name), builder); }
        public static bool RegisterBuilder(int id, IBrickMeshBuilder builder) { return RegisterBuilder(BlockTypeInfo.GetTypeInfo(id), builder); }

        public static bool RegisterBuilder(BlockTypeInfo typeInfo, IBrickMeshBuilder builder)
        {
            if (typeInfo != null && builder != null)
            {
                var visualInfo = _brickVisualRegistry.GetItem(typeInfo.ID);
                if (visualInfo == null)
                    visualInfo = new BlockVisualInfo(typeInfo);

                visualInfo.Builder = builder;
                return true;
            }
            return false;
        }

        //TODO: Useful parameters once we need them.
        public BlockVisualInfo(BlockTypeInfo typeInfo)
        {
            BrickType = typeInfo;

            _brickVisualRegistry.RegisterItem(this, typeInfo.Name, typeInfo.ID);
        }
    }
}

