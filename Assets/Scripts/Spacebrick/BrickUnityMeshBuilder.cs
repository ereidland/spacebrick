//
// BrickUnityMeshBuilder.cs
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
    public class BrickUnityMeshBuilder : IBrickMeshBuilder
    {
        public int TotalVertices
        {
            get { return _vertices.Length; }
        }

        public int TotalIndices
        {
            get { return _indices.Length; }
        }

        public Material RenderMaterial { get; private set; }

        public Mesh RenderMesh { get; private set; }

        //TODO: Automatic face culling eventually.
        //Possibly by looking at the faces that are either at or "really close" to the edges of the boundaries.
        //Then groups of vertex and index information would be split by the face they belong to.

        private int[] _indices;
        private Vector3[] _vertices;
        private Vector3[] _normals;
        private Vector2[] _uvs;
        private Color[] _colors;

        public void Build(MeshBuilder builder, Brick brick)
        {
            Vector3 rootPosition = new Vector3(brick.Rect.X, brick.Rect.Y, brick.Rect.Z);
            Vector3 blockScale = new Vector3(brick.Rect.Width, brick.Rect.Height, brick.Rect.Depth);
            Vector3 centerPosition = rootPosition + blockScale*0.5f;

            builder.PrepIndices(RenderMaterial, _indices.Length);
            int indexOffset = builder.CurrentVertex;
            Quaternion rotation = brick.Info.Direction.ToRotation();

            for (int i = 0; i < _vertices.Length; i++)
                builder.AddVertex(centerPosition + Vector3.Scale(rotation*_vertices[i], blockScale), _normals[i], _uvs[i], _colors[i]);

            for (int i = 0; i < _indices.Length; i++)
                builder.AddIndex(_indices[i] + indexOffset);
        }

        public void OverrideColor(Color color)
        {
            for (int i = 0; i < _colors.Length; i++)
                _colors[i] = color;
        }

        public BrickUnityMeshBuilder(Mesh mesh, Material material)
        {
            RenderMaterial = material;
            RenderMesh = mesh;

            _indices = mesh.GetIndices(0);
            _vertices = mesh.vertices;
            _normals = mesh.normals;
            _uvs = mesh.uv;
            _colors = mesh.colors;
        }
    }
}

