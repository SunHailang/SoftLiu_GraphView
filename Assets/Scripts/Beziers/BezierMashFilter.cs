using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class BezierMashFilter : MonoBehaviour
{
    public MeshFilter _MeshFilter;
    public MeshRenderer _MeshRenderer;
    public int SegmentNum;
    public float Width;

    public List<Transform> trans = new List<Transform>();

    private Vector3[] _vertices;
    private int[] _indices;
    private int[] _triangles;

    private List<Vector3> points = new List<Vector3>();
    private List<Vector3> pointsMax = null;
    private List<Vector3> pointsMin = null;

    private Vector3[] getPoints()
    {
        points.Clear();
        foreach (var t in trans)
        {
            if (t == null) continue;
            points.Add(t.position);
        }

        return BezierUtils.GetLineBeizerList(points, SegmentNum);
    }

    private void Update()
    {
        BezierPoints();
    }

    private void BezierPoints()
    {
        var paths = getPoints();

        pointsMax = new List<Vector3>(paths.Length);
        pointsMin = new List<Vector3>(paths.Length);

        _vertices = new Vector3[paths.Length * 2];
        _indices = new int[paths.Length * 2];

        int index = 0;
        // 计算三角形
        int trianglesCount = (paths.Length - 1) * 2 * 3;
        _triangles = new int[trianglesCount];
        for (int i = 0; i < paths.Length - 1; i++)
        {
            Vector3 dir1 = (paths[i + 1] - paths[i]).normalized;
            Vector3 dirMin = new Vector3(-dir1.z, 0, dir1.x);
            Vector3 dirMax = new Vector3(dir1.z, 0, -dir1.x);
            Ray rayMax = new Ray(paths[i], dirMax);
            Vector3 max = rayMax.GetPoint(Width / 2);
            Ray rayMin = new Ray(paths[i], dirMin);
            Vector3 min = rayMin.GetPoint(Width / 2);

            pointsMax.Add(max);
            pointsMin.Add(min);

            _vertices[index] = max;
            _indices[index] = index;
            index++;
            _vertices[index] = min;
            _indices[index] = index;
            index++;

            if (i == paths.Length - 2)
            {
                rayMax = new Ray(paths[i + 1], dirMax);
                max = rayMax.GetPoint(Width / 2);
                rayMin = new Ray(paths[i + 1], dirMin);
                min = rayMin.GetPoint(Width / 2);

                pointsMax.Add(max);
                pointsMin.Add(min);

                _vertices[index] = max;
                _indices[index] = index;
                index++;
                _vertices[index] = min;
                _indices[index] = index;
                index++;
            }
            
            int indexT = i * 2;
            _triangles[6 * i] = indexT;
            _triangles[6 * i + 1] = indexT + 1;
            _triangles[6 * i + 2] = indexT + 2;

            _triangles[6 * i + 3] = indexT + 2;
            _triangles[6 * i + 4] = indexT + 1;
            _triangles[6 * i + 5] = indexT + 3;
        }

        Mesh mesh = new Mesh();
        mesh.name = "BeziersMesh";
        _MeshFilter.sharedMesh = mesh;

        mesh.vertices = _vertices;
        mesh.triangles = _triangles;
        // mesh.SetIndices(_indices, MeshTopology.Points, 0);
        mesh.RecalculateNormals();
        mesh.RecalculateBounds();

        var material = new Material(Shader.Find("Diffuse"));
        material.SetColor("_Color", Color.yellow);

        _MeshRenderer.sharedMaterial = material;
        
        // 提取Mesh
        // SaveMesh(mesh);
    }

    private void SaveMesh(Mesh mesh)
    {
        if (mesh != null)
        {
            AssetDatabase.CreateAsset(mesh, "Assets/SaveMesh.asset");
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }
    }

    private void Start()
    {
        BezierPoints();
        //GetTriangle();
    }


    private GameObject GetTriangle()
    {
        var go = new GameObject("Triangle");
        var filter = go.AddComponent<MeshFilter>();

        var mesh = new Mesh();
        filter.sharedMesh = mesh;
        mesh.vertices = new[]
        {
            new Vector3(0, 10, 0),
            new Vector3(20, -10, 0),
            new Vector3(-20, -10, 0)
        };

        mesh.triangles = new int[3] {0, 1, 2};
        mesh.RecalculateNormals();
        mesh.RecalculateBounds();

        var material = new Material(Shader.Find("Diffuse"));
        material.SetColor("_Color", Color.yellow);

        var renderer = go.AddComponent<MeshRenderer>();
        renderer.sharedMaterial = material;
        return go;
    }

    private void OnDrawGizmos()
    {
        if (_vertices != null)
        {
            for (int i = 0; i < _vertices.Length; i++)
            {
                Gizmos.color = Color.white;
                Gizmos.DrawSphere(_vertices[i], 2);
            }
        }
    }
}