//
// BrickPrefabConfig.cs
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
using UnityEngine;
using System.Collections;

namespace Spacebrick
{
    public class BlockPrefabConfig : MonoBehaviour
    {
        public enum MeshSettings
        {
            LeaveItAlone,
            OverrideColor,
        }

        [SerializeField]
        private string _name;

        [SerializeField]
        private Mesh _mesh;

        [SerializeField]
        private MeshSettings _meshSettings = MeshSettings.LeaveItAlone;

        [SerializeField]
        private Material _material;

        [SerializeField]
        private Color _color = Color.white;

        [SerializeField]
        private bool _hasDirection = true;

        public BlockTypeInfo TypeInfo { get; private set; }

        public void Register()
        {
            TypeInfo = BlockTypeInfo.GetTypeInfo(name);
            if (TypeInfo == null)
                TypeInfo = new BlockTypeInfo(_name, _hasDirection);

            var builder = new BrickUnityMeshBuilder(_mesh, _material);

            if (_meshSettings == MeshSettings.OverrideColor)
                builder.OverrideColor(_color);

            BlockVisualInfo.RegisterBuilder(TypeInfo, builder);
        }
    }
}
