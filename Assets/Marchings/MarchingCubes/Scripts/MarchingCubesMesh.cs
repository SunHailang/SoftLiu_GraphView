using System;
using System.Collections.Generic;
using Marchings.MarchingSquares;
using Unity.Mathematics;
using UnityEngine;

namespace Marchings.MarchingCubes
{
    [RequireComponent(typeof(MeshFilter), typeof(MeshRenderer), typeof(MeshCollider))]
    public class MarchingCubesMesh : MonoBehaviour
    {
        public MeshFilter _meshFilter;
        public MeshRenderer _meshRenderer;
        public MeshCollider _meshCollider;
        
        [Header("单个空间的大小")]
        public Vector3Int MapSize;

        public Vector3 CubeSize;
        public float NoiseScale;

        public Vector2 layerPerlinHeightRange = new Vector2(-1, 1);
        [Min(0)] public float cellSize = 1f;
        [Min(0)] public int octaves = 4;
        [Min(0)] public float roughness = 3;
        [Min(0)] public float persistence = 0.4f;

        private List<MarchingCube> mapList;

        private void Start()
        {
            // 1. 获取空间坐标点
            
        }

        private void Generate()
        {
            mapList = new List<MarchingCube>();
            ProceduralMeshPart main = new ProceduralMeshPart();
            
            for (int x = 0; x < MapSize.x; x++)
            {
                for (int z = 0; z < MapSize.z; z++)
                {
                    for (int y = 0; y < MapSize.y; y++)
                    {
                        var cube = CreateMarchingCube(new Vector3(x * CubeSize.x, y * CubeSize.y, z * CubeSize.z), CubeSize * 0.5f);
                        cube.BuildMesh(main, 0.5f, true);
                    }
                }
            }
            main.FillArrays();
            var mesh = new Mesh();
            _meshFilter.sharedMesh = mesh;
            mesh.vertices = main.Vertices;
            mesh.uv = main.UVs;
            mesh.triangles = main.Triangles;
            mesh.colors = main.Colors;
            mesh.RecalculateNormals();
        }

        private MarchingCube CreateMarchingCube(Vector3 centerPos, Vector3 size)
        {
            Vector3[] corner;
            float[] values;

            corner = new Vector3[8];
            corner[0] = centerPos + new Vector3(-0.5f, -0.5f, 0.5f);
            corner[1] = centerPos + new Vector3(0.5f, -0.5f, 0.5f);
            corner[2] = centerPos + new Vector3(0.5f, -0.5f, -0.5f);
            corner[3] = centerPos + new Vector3(-0.5f, -0.5f, -0.5f);
            corner[4] = centerPos + new Vector3(-0.5f, 0.5f, 0.5f);
            corner[5] = centerPos + new Vector3(0.5f, 0.5f, 0.5f);
            corner[6] = centerPos + new Vector3(0.5f, 0.5f, -0.5f);
            corner[7] = centerPos + new Vector3(-0.5f, 0.5f, -0.5f);
            
            values = new float[8];
            for (int i = 0; i < 8; i++)
            {
                //values[i] = Vector3.SqrMagnitude(corner[i] - radius * Vector3.one);
                values[i] = Unity.Mathematics.noise.cnoise(new float3(corner[i].x, corner[i].y, corner[i].z) * NoiseScale);
            }
            
            return new MarchingCube(centerPos, corner, values);
        }

    }
}