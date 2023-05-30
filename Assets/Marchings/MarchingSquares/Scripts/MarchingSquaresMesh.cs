using System;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Marchings.MarchingSquares
{
    [RequireComponent(typeof(MeshFilter), typeof(MeshRenderer), typeof(MeshCollider))]
    public class MarchingSquaresMesh : MonoBehaviour
    {
        public MeshFilter _meshFilter;
        public MeshRenderer _meshRenderer;
        public MeshCollider _meshCollider;

        public int squareSizeX;
        public int squareSizeY;

        [Range(1.0f, 5.0f)] public float squareSize;

        [Range(-1f, 1.0f)] public float Threshold;
        public bool SquareLerp;
        public bool IsBuild3D;
        [Range(1f, 3f)] public float Height3D;

        private MarchingSquare[] _squaresMap;

        private float _perThreshold = 0f;

        private void Start()
        {
            Random.InitState(123);

            //Generate();
        }

        private void Update()
        {
            if (Math.Abs(_perThreshold - Threshold) > 0.001f)
            {
                Generate();
                _perThreshold = Threshold;
            }
        }

        private void Generate()
        {
            int length = squareSizeX * squareSizeY;
            float halfSize = squareSize * 0.5f;

            _squaresMap = new MarchingSquare[length + 1];

            var main = new ProceduralMeshPart();

            for (int i = 0; i < squareSizeX; i++)
            {
                for (int j = 0; j < squareSizeY; j++)
                {
                    int index = i * squareSizeY + j;
                    _squaresMap[index] = new MarchingSquare(
                        new Vector3(i * squareSize + halfSize, 0, j * squareSize + halfSize),
                        Threshold,
                        halfSize,
                        SquareLerp
                    );
                    if (IsBuild3D)
                    {
                        _squaresMap[index].BuildMashTo3D(main, Height3D);
                    }
                    else
                    {
                        _squaresMap[index].BuildMashTo2D(main);
                    }
                }
            }

            if (IsBuild3D)
            {
                // 底板
                main.AddQuad(Vector3.zero, new Vector3(0, 0, squareSizeY), new Vector3(squareSizeX, 0, squareSizeY),
                    new Vector3(squareSizeX, 0, 0));
                main.AddQuad(Vector3.zero, new Vector3(squareSizeX, 0, 0), new Vector3(squareSizeX, 0, squareSizeY),
                    new Vector3(0, 0, squareSizeY));
                // 左
                main.AddQuad(Vector3.zero, new Vector3(0, Height3D, 0), new Vector3(0, Height3D, squareSizeY), new Vector3(0, 0, squareSizeY));
                // 上
                main.AddQuad(new Vector3(0, 0, squareSizeY), new Vector3(0, Height3D, squareSizeY), new Vector3(squareSizeX, Height3D, squareSizeY),
                    new Vector3(squareSizeX, 0, squareSizeY));
                // 右
                main.AddQuad(new Vector3(squareSizeX, 0, squareSizeY), new Vector3(squareSizeX, Height3D, squareSizeY), new Vector3(squareSizeX, Height3D, 0),
                    new Vector3(squareSizeX, 0, 0));
                // 下
                main.AddQuad(new Vector3(squareSizeX, 0, 0), new Vector3(squareSizeX, Height3D, 0), new Vector3(0, Height3D, 0),
                    Vector3.zero);
            }

            main.FillArrays();
            var mesh = new Mesh();
            _meshFilter.sharedMesh = mesh;
            mesh.vertices = main.Vertices;
            mesh.uv = main.UVs;
            mesh.triangles = main.Triangles;
            mesh.colors = main.Colors;
            mesh.RecalculateNormals();

            var mat = new Material(Shader.Find("Diffuse"));
            mat.SetColor("_Color", Color.yellow);
            _meshRenderer.sharedMaterial = mat;
            ConfigCollider();
        }

        private void ConfigCollider()
        {
            _meshCollider.sharedMesh = _meshFilter.mesh;
        }
    }
}