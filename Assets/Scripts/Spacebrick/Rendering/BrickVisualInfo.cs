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

        public static Quaternion ToRotation(this BrickDirection direciton)
        {
            switch (direciton)
            {
                case BrickDirection.Right:
                    return Right;
                case BrickDirection.Back:
                    return Back;
                case BrickDirection.Left:
                    return Left;
                case BrickDirection.Down:
                    return Down;
                case BrickDirection.Up:
                    return Up;
                case BrickDirection.Forward:
                default:
                    return Forward;
            }
        }
    }
    public class BrickVisualInfo
    {
        private static NameIDRegistry<BrickVisualInfo> _brickVisualRegistry = new NameIDRegistry<BrickVisualInfo>();

        public BrickTypeInfo BrickType { get; private set; }
        public IBrickMeshBuilder Builder { get; private set; }

        public static BrickVisualInfo GetVisualInfo(string name) { return _brickVisualRegistry.GetItem(name); }
        public static BrickVisualInfo GetVisualInfo(int id) { return _brickVisualRegistry.GetItem(id); }

        public static bool RegisterBuilder(string name, IBrickMeshBuilder builder) { return RegisterBuilder(BrickTypeInfo.GetTypeInfo(name), builder); }
        public static bool RegisterBuilder(int id, IBrickMeshBuilder builder) { return RegisterBuilder(BrickTypeInfo.GetTypeInfo(id), builder); }

        public static bool RegisterBuilder(BrickTypeInfo typeInfo, IBrickMeshBuilder builder)
        {
            if (typeInfo != null && builder != null)
            {
                var visualInfo = _brickVisualRegistry.GetItem(typeInfo.ID);
                if (visualInfo == null)
                    visualInfo = new BrickVisualInfo(typeInfo);

                visualInfo.Builder = builder;
                return true;
            }
            return false;
        }

        //TODO: Useful parameters once we need them.
        public BrickVisualInfo(BrickTypeInfo typeInfo)
        {
            BrickType = typeInfo;

            _brickVisualRegistry.RegisterItem(this, typeInfo.Name, typeInfo.ID);
        }
    }
}

