//
// BrickChunkMeshBuilder.cs
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
    public class BrickChunkRenderer : MonoBehaviour
    {
        private MeshBuilder _builder = new MeshBuilder();
        private List<MeshFilter> _meshFilters = new List<MeshFilter>();

        public bool BakeColliders = false;

        private class BrickBuildInfo
        {
            public Brick Brick;
            public BlockVisualInfo VisualInfo;

            public BrickBuildInfo(Brick brick, BlockVisualInfo visualInfo)
            {
                Brick = brick;
                VisualInfo = visualInfo;
            }
        }


        public BrickChunk AssignedChunk { get; set; }

        private void AllocateMeshes(int howMany)
        {
            if (_meshFilters.Count < howMany)
            {
                for (int i = _meshFilters.Count; i < howMany; i++)
                {
                    var filter = new GameObject("mesh_" + i).AddComponent<MeshFilter>();
                    filter.transform.parent = transform;
                    filter.transform.localPosition = Vector3.zero;
                    filter.transform.localRotation = Quaternion.identity;

                    filter.gameObject.AddComponent<MeshRenderer>();

                    _meshFilters.Add(filter);
                }

                for (int i = howMany - 1; i >= _meshFilters.Count; i--)
                {
                    var filter = _meshFilters[i];
                    if (filter != null)
                    {
                        StaticMeshPool.ReleaseMesh(filter.sharedMesh);
                        Destroy(filter.gameObject);
                    }
                    _meshFilters.RemoveAt(i);
                }
            }
        }

        public void BuildMeshes(BrickChunk chunk)
        {
            _builder.Begin();
            var buildInfo = new BrickBuildInfo[chunk.BrickCount];
            int buildIndex = 0;

            foreach (var brick in chunk.Bricks)
            {
                var visual = BlockVisualInfo.GetVisualInfo(brick.Info.ID);
                if (visual != null)
                {
                    if (visual.Builder != null)
                    {
                        _builder.AddToAllocation(visual.Builder.RenderMaterial, visual.Builder.TotalIndices, visual.Builder.TotalVertices);
                        buildInfo[buildIndex++] = new BrickBuildInfo(brick, visual);
                    }
                    else
                        Debug.LogError("No builder for brick " + brick.Info.ID);
                }
                else
                    Debug.LogError("No visual for brick " + brick.Info.ID);
            }

            _builder.DoAllocation();

            foreach (var brick in chunk.Bricks)
            {
                var visual = BlockVisualInfo.GetVisualInfo(brick.Info.ID);
                if (visual != null && visual.Builder != null)
                    visual.Builder.Build(_builder, brick);
            }

            var meshes = _builder.GenerateMeshes();
            AllocateMeshes(meshes.Length);
            for (int i = 0; i < meshes.Length; i++)
            {
                var meshFilter = _meshFilters[i];
                var sharedMesh = meshFilter.sharedMesh;
                if (sharedMesh != null)
                    StaticMeshPool.ReleaseMesh(sharedMesh);

                var generatedMeshInfo = meshes[i];

                meshFilter.sharedMesh = generatedMeshInfo.GeneratedMesh;
                meshFilter.renderer.sharedMaterial = generatedMeshInfo.RenderMaterial;

                if (BakeColliders)
                {
                    var meshCollider = meshFilter.GetComponent<MeshCollider>();
                    if (meshCollider == null)
                        meshCollider = meshFilter.gameObject.AddComponent<MeshCollider>();
                    else
                    {
                        meshCollider.sharedMesh = null;
                        meshCollider.sharedMesh = generatedMeshInfo.GeneratedMesh;
                    }
                }
            }
        }

        public void BuildMeshes()
        {
            if (AssignedChunk != null)
                BuildMeshes(AssignedChunk);
        }
    }
}

