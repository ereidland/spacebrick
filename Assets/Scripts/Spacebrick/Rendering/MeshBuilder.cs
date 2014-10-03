//
// MeshBuilder.cs
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
using UnityEngine;

namespace Spacebrick
{
    //A lot of values are not protected against null reference exceptions... That'll need to change.
    public class MeshBuilder
    {
        public const int IndexLimit = 65000; //Not sure why I can't have the other 535 in Unity.

        private class MeshInfo
        {
            public int[] Indices;
            public Vector3[] Vertices;
            public Vector3[] Normals;
            public Vector2[] UVs;
            public Color[] Colors;

            public int CurrentVertex;
            public int CurrentIndex;

            public bool CanHoldIndices(int indices) { return CurrentIndex + indices < IndexLimit; }

            public void ApplyToMesh(Mesh mesh)
            {
                mesh.vertices = Vertices;
                mesh.normals = Normals;
                mesh.uv = UVs;
                mesh.colors = Colors;
                mesh.SetIndices(Indices, MeshTopology.Triangles, 0);
            }

            public void AddIndex(int index) { Indices[CurrentIndex++] = index; }

            public void AddVertex(Vector3 position, Vector3 normal, Vector2 uv, Color color)
            {
                Vertices[CurrentVertex] = position;
                Normals[CurrentVertex] = normal;
                UVs[CurrentVertex] = uv;
                Colors[CurrentVertex] = color;

                CurrentVertex++;
            }

            public void Allocate(int indexCount, int vertexCount)
            {
                Indices = new int[indexCount];
                Vertices = new Vector3[vertexCount];
                Normals = new Vector3[vertexCount];
                UVs = new Vector2[vertexCount];
                Colors = new Color[vertexCount];
            }
        }

        private class MeshChannel
        {
            public List<MeshInfo> MeshInfoList = new List<MeshInfo>();
            public Material RenderMaterial { get; private set; } 
            public MeshInfo TopInfo;
            public int TopInfoIndex;

            public void CreateNextInfo()
            {
                TopInfo = new MeshInfo();
                MeshInfoList.Add(TopInfo);
                TopInfoIndex = MeshInfoList.Count - 1;
            }

            public void GoToNextInfo()
            {
                TopInfo = MeshInfoList[++TopInfoIndex];
            }

            public MeshChannel(Material renderMaterial)
            {
                RenderMaterial = renderMaterial;
                TopInfo = new MeshInfo();
                MeshInfoList.Add(TopInfo);
            }
        }

        public class GeneratedMeshInfo
        {
            public Mesh GeneratedMesh { get; private set; }
            public Material RenderMaterial { get; private set; }
            public GeneratedMeshInfo(Mesh mesh, Material material)
            {
                GeneratedMesh = mesh;
                RenderMaterial = material;
            }
        }

        private List<MeshChannel> _meshChannels = new List<MeshChannel>();

        private MeshChannel _currentChannel;
        private MeshInfo _currentInfo { get { return _currentChannel.TopInfo; } }

        public int CurrentIndex { get { return _currentInfo.CurrentVertex; } }
        public int CurrentVertex { get { return _currentInfo.CurrentVertex; } }

        public void Begin()
        {
            _meshChannels.Clear();
        }

        public void AddToAllocation(Material material, int indices, int vertices)
        {
            _currentChannel = FindOrCreateChannel(material);
            if (!_currentChannel.TopInfo.CanHoldIndices(indices))
                _currentChannel.CreateNextInfo();

            _currentInfo.CurrentIndex += indices;
            _currentInfo.CurrentVertex += vertices;
        }

        public void DoAllocation()
        {
            foreach (var meshChannel in _meshChannels)
            {
                foreach (var meshInfo in meshChannel.MeshInfoList)
                {
                    meshInfo.Allocate(meshInfo.CurrentIndex, meshInfo.CurrentVertex);
                    meshInfo.CurrentIndex = 0;
                    meshInfo.CurrentVertex = 0;
                }

                meshChannel.TopInfoIndex = 0;
                meshChannel.TopInfo = meshChannel.MeshInfoList[0];
            }

            _currentChannel = null;
        }

        private MeshChannel FindOrCreateChannel(Material material)
        {
            for (int i = 0; i < _meshChannels.Count; i++)
            {
                var meshChannel = _meshChannels[i];
                if (meshChannel.RenderMaterial == material)
                    return meshChannel;
            }

            var newChannel = new MeshChannel(material);
            _meshChannels.Add(newChannel);
            return newChannel;
        }

        public void PrepIndices(Material material, int count)
        {
            _currentChannel = FindOrCreateChannel(material);

            if (!_currentChannel.TopInfo.CanHoldIndices(count))
                _currentChannel.GoToNextInfo();
        }

        public void AddIndex(int index)
        {
            _currentInfo.AddIndex(index);
        }

        public void AddVertex(Vector3 position, Vector3 normal, Vector2 uv, Color color)
        {
            _currentInfo.AddVertex(position, normal, uv, color);
        }

        public GeneratedMeshInfo[] GenerateMeshes()
        {
            int totalCount = 0;
            for (int i = 0; i < _meshChannels.Count; i++)
                totalCount += _meshChannels[i].MeshInfoList.Count;

            GeneratedMeshInfo[] meshInfoArray = new GeneratedMeshInfo[totalCount];
            int generatedInfoIndex = 0;

            for (int i = 0; i < _meshChannels.Count; i++)
            {
                var meshChannel = _meshChannels[i];
                for (int j = 0; j < meshChannel.MeshInfoList.Count; j++)
                {
                    var meshInfo = meshChannel.MeshInfoList[j];
                    var mesh = StaticMeshPool.GetMesh();
                    meshInfo.ApplyToMesh(mesh);
                    meshInfoArray[generatedInfoIndex++] = new GeneratedMeshInfo(mesh, meshChannel.RenderMaterial);
                }
            }

            return meshInfoArray;
        }
    }
}