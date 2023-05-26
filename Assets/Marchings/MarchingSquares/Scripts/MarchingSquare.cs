using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Marchings.MarchingSquares
{
    public class MarchingSquare
    {
        public Vector3 CenterPoint;
        public MarchingSquarePoint[] CornerPoints;

        public Vector3[] MidPoints;
        public float Threshold = 0.5f;

        private float GetNoise(float x, float y)
        {
            return Unity.Mathematics.noise.snoise(new Unity.Mathematics.float2(x, y));
        }

        public MarchingSquare(Vector3 center, float threshold, float halfSize, bool squareLerp)
        {
            CenterPoint = center;
            Threshold = threshold;
            CornerPoints = new MarchingSquarePoint[4];
            var lbPos = center + halfSize * new Vector3(-1, 0, -1);
            var ltPos = center + halfSize * new Vector3(-1, 0, 1);
            var rtPos = center + halfSize * new Vector3(1, 0, 1);
            var rbPos = center + halfSize * new Vector3(1, 0, -1);
            CornerPoints[0] = new MarchingSquarePoint(lbPos, GetNoise(lbPos.x, lbPos.z));
            CornerPoints[1] = new MarchingSquarePoint(ltPos, GetNoise(ltPos.x, ltPos.z));
            CornerPoints[2] = new MarchingSquarePoint(rtPos, GetNoise(rtPos.x, rtPos.z));
            CornerPoints[3] = new MarchingSquarePoint(rbPos, GetNoise(rbPos.x, rbPos.z));
            MidPoints = new Vector3[4];
            if (squareLerp)
            {
                var lbTolt = CornerPoints[0].Value / (CornerPoints[0].Value + CornerPoints[1].Value);
                MidPoints[0] = Vector3.Lerp(ltPos, lbPos, lbTolt);

                var ltTort = CornerPoints[1].Value / (CornerPoints[1].Value + CornerPoints[2].Value);
                MidPoints[1] = Vector3.Lerp(rtPos, ltPos, ltTort);

                var rtTorb = CornerPoints[2].Value / (CornerPoints[2].Value + CornerPoints[3].Value);
                MidPoints[2] = Vector3.Lerp(rbPos, rtPos, rtTorb);

                var rbTolb = CornerPoints[3].Value / (CornerPoints[3].Value + CornerPoints[0].Value);
                MidPoints[3] = Vector3.Lerp(lbPos, rbPos, rbTolb);
            }
            else
            {
                MidPoints[0] = center + halfSize * new Vector3(-1, 0, 0);
                MidPoints[1] = center + halfSize * new Vector3(0, 0, 1);
                MidPoints[2] = center + halfSize * new Vector3(1, 0, 0);
                MidPoints[3] = center + halfSize * new Vector3(0, 0, -1);
            }
        }

        public void BuildMashTo3D(ProceduralMeshPart meshPart, float height)
        {
            int bitMask = GetBitMaskValue();
            switch (bitMask)
            {
                case 0: break;
                case 1:
                    meshPart.AddTriangle(CornerPoints[0].Position, MidPoints[0], MidPoints[3], height);
                    break;
                case 2:
                    meshPart.AddTriangle(CornerPoints[1].Position, MidPoints[1], MidPoints[0], height);
                    break;
                case 3:
                    meshPart.AddQuad(CornerPoints[0].Position, CornerPoints[1].Position, MidPoints[1], MidPoints[3], height);
                    break;
                case 4:
                    meshPart.AddTriangle(CornerPoints[2].Position, MidPoints[2], MidPoints[1], height);
                    break;
                case 5:
                    meshPart.AddTriangle(CornerPoints[0].Position, MidPoints[0], MidPoints[3], height);
                    meshPart.AddQuad(MidPoints[3], MidPoints[0], MidPoints[1], MidPoints[2], height);
                    meshPart.AddTriangle(MidPoints[1], CornerPoints[2].Position, MidPoints[2], height);
                    break;
                case 6:
                    meshPart.AddQuad(MidPoints[0], CornerPoints[1].Position, CornerPoints[2].Position, MidPoints[2], height);
                    break;
                case 7:
                    meshPart.AddPentagon(CornerPoints[2].Position, MidPoints[2], MidPoints[3], CornerPoints[0].Position, CornerPoints[1].Position, height);
                    break;
                case 8:
                    meshPart.AddTriangle(MidPoints[2], CornerPoints[3].Position, MidPoints[3], height);
                    break;
                case 9:
                    meshPart.AddQuad(CornerPoints[0].Position, MidPoints[0], MidPoints[2], CornerPoints[3].Position, height);
                    break;
                case 10:
                    meshPart.AddTriangle(MidPoints[0], CornerPoints[1].Position, MidPoints[1], height);
                    meshPart.AddQuad(MidPoints[0], MidPoints[1], MidPoints[2], MidPoints[3], height);
                    meshPart.AddTriangle(MidPoints[2], CornerPoints[3].Position, MidPoints[3], height);
                    break;
                case 11:
                    meshPart.AddPentagon(CornerPoints[0].Position, CornerPoints[1].Position, MidPoints[1], MidPoints[2], CornerPoints[3].Position, height);
                    break;
                case 12:
                    meshPart.AddQuad(MidPoints[3], MidPoints[1], CornerPoints[2].Position, CornerPoints[3].Position, height);
                    break;
                case 13:
                    meshPart.AddPentagon(CornerPoints[0].Position, MidPoints[0], MidPoints[1], CornerPoints[2].Position, CornerPoints[3].Position, height);
                    break;
                case 14:
                    meshPart.AddPentagon(CornerPoints[1].Position, CornerPoints[2].Position, CornerPoints[3].Position, MidPoints[3], MidPoints[0], height);
                    break;
                case 15:
                    meshPart.AddQuad(CornerPoints[0].Position, CornerPoints[1].Position, CornerPoints[2].Position, CornerPoints[3].Position, height);
                    break;
            }
        }

        public void BuildMashTo2D(ProceduralMeshPart meshPart)
        {
            int bitMask = GetBitMaskValue();
            switch (bitMask)
            {
                case 0: break;
                case 1:
                    meshPart.AddTriangle(CornerPoints[0].Position, MidPoints[0], MidPoints[3]);
                    break;
                case 2:
                    meshPart.AddTriangle(CornerPoints[1].Position, MidPoints[1], MidPoints[0]);
                    break;
                case 3:
                    meshPart.AddQuad(CornerPoints[0].Position, CornerPoints[1].Position, MidPoints[1], MidPoints[3]);
                    break;
                case 4:
                    meshPart.AddTriangle(CornerPoints[2].Position, MidPoints[2], MidPoints[1]);
                    break;
                case 5:
                    meshPart.AddTriangle(CornerPoints[0].Position, MidPoints[0], MidPoints[3]);
                    meshPart.AddQuad(MidPoints[3], MidPoints[0], MidPoints[1], MidPoints[2]);
                    meshPart.AddTriangle(MidPoints[1], CornerPoints[2].Position, MidPoints[2]);
                    break;
                case 6:
                    meshPart.AddQuad(MidPoints[0], CornerPoints[1].Position, CornerPoints[2].Position, MidPoints[2]);
                    break;
                case 7:
                    meshPart.AddPentagon(CornerPoints[2].Position, MidPoints[2], MidPoints[3], CornerPoints[0].Position, CornerPoints[1].Position);
                    break;
                case 8:
                    meshPart.AddTriangle(MidPoints[2], CornerPoints[3].Position, MidPoints[3]);
                    break;
                case 9:
                    meshPart.AddQuad(CornerPoints[0].Position, MidPoints[0], MidPoints[2], CornerPoints[3].Position);
                    break;
                case 10:
                    meshPart.AddTriangle(MidPoints[0], CornerPoints[1].Position, MidPoints[1]);
                    meshPart.AddQuad(MidPoints[0], MidPoints[1], MidPoints[2], MidPoints[3]);
                    meshPart.AddTriangle(MidPoints[2], CornerPoints[3].Position, MidPoints[3]);
                    break;
                case 11:
                    meshPart.AddPentagon(CornerPoints[0].Position, CornerPoints[1].Position, MidPoints[1], MidPoints[2], CornerPoints[3].Position);
                    break;
                case 12:
                    meshPart.AddQuad(MidPoints[3], MidPoints[1], CornerPoints[2].Position, CornerPoints[3].Position);
                    break;
                case 13:
                    meshPart.AddPentagon(CornerPoints[0].Position, MidPoints[0], MidPoints[1], CornerPoints[2].Position, CornerPoints[3].Position);
                    break;
                case 14:
                    meshPart.AddPentagon(CornerPoints[1].Position, CornerPoints[2].Position, CornerPoints[3].Position, MidPoints[3], MidPoints[0]);
                    break;
                case 15:
                    meshPart.AddQuad(CornerPoints[0].Position, CornerPoints[1].Position, CornerPoints[2].Position, CornerPoints[3].Position);
                    break;
            }
        }

        private int GetBitMaskValue()
        {
            var lbValue = CornerPoints[0].Value > Threshold ? 1 : 0;
            var ltValue = CornerPoints[1].Value > Threshold ? 2 : 0;
            var rtValue = CornerPoints[2].Value > Threshold ? 4 : 0;
            var rbValue = CornerPoints[3].Value > Threshold ? 8 : 0;
            return lbValue + ltValue + rtValue + rbValue;
        }
    }

    public class MarchingSquarePoint
    {
        public Vector3 Position;
        public readonly float Value;

        public MarchingSquarePoint(Vector3 pos, float noiseValue)
        {
            Position = pos;
            Value = noiseValue;
        }
    }

    public class ProceduralMeshPart
    {
        public Vector3[] Vertices;
        public Vector2[] UVs;
        public int[] Triangles;
        public Color[] Colors;

        private List<Vector3> _vertices = new List<Vector3>();
        private List<int> _triangles = new List<int>();
        private List<Vector2> _uvs = new List<Vector2>();
        private List<Color> _colors = new List<Color>();

        public void FillArrays()
        {
            Vertices = _vertices.ToArray();
            UVs = _uvs.ToArray();
            Triangles = _triangles.ToArray();
            Colors = _colors.ToArray();
        }

        public ProceduralMeshPart()
        {
        }

        public ProceduralMeshPart(Vector3[] vertices, int[] triangles, Vector2[] uvs, Color[] colors)
        {
            Vertices = vertices;
            UVs = uvs;
            Triangles = triangles;
            Colors = colors;
        }

        public void AddMeshPart(ProceduralMeshPart meshPart)
        {
            for (int i = 0; i < meshPart.Vertices.Length; i++)
            {
                _vertices.Add(meshPart.Vertices[i]);
            }

            for (int i = 0; i < meshPart.UVs.Length; i++)
            {
                _uvs.Add(meshPart.UVs[i]);
            }

            for (int i = 0; i < meshPart.Triangles.Length; i++)
            {
                _triangles.Add(meshPart.Triangles[i]);
            }

            for (int i = 0; i < meshPart.Colors.Length; i++)
            {
                _colors.Add(meshPart.Colors[i]);
            }
        }

        public void AddTriangle(Vector3 point1, Vector3 point2, Vector3 point3)
        {
            int vertexIndex = _vertices.Count;
            _vertices.Add(point1);
            _vertices.Add(point2);
            _vertices.Add(point3);
            _triangles.Add(vertexIndex);
            _triangles.Add(vertexIndex + 1);
            _triangles.Add(vertexIndex + 2);
        }

        public void AddQuad(Vector3 lb, Vector3 lt, Vector3 rt, Vector3 rb)
        {
            int vertexIndex = _vertices.Count;
            _vertices.Add(lb);
            _vertices.Add(lt);
            _vertices.Add(rt);
            _vertices.Add(rb);
            _triangles.Add(vertexIndex);
            _triangles.Add(vertexIndex + 1);
            _triangles.Add(vertexIndex + 2);
            _triangles.Add(vertexIndex);
            _triangles.Add(vertexIndex + 2);
            _triangles.Add(vertexIndex + 3);
        }

        public void AddPentagon(Vector3 a, Vector3 b, Vector3 c, Vector3 d, Vector3 e)
        {
            int vertexIndex = _vertices.Count;
            _vertices.Add(a);
            _vertices.Add(b);
            _vertices.Add(c);
            _vertices.Add(d);
            _vertices.Add(e);
            _triangles.Add(vertexIndex);
            _triangles.Add(vertexIndex + 1);
            _triangles.Add(vertexIndex + 2);
            _triangles.Add(vertexIndex);
            _triangles.Add(vertexIndex + 2);
            _triangles.Add(vertexIndex + 3);
            _triangles.Add(vertexIndex);
            _triangles.Add(vertexIndex + 3);
            _triangles.Add(vertexIndex + 4);
        }

        public void AddTriangle(Vector3 point1, Vector3 point2, Vector3 point3, float height)
        {
            var h = new Vector3(0, height, 0);
            var point1h = point1 + h;
            var point2h = point2 + h;
            var point3h = point3 + h;

            //AddTriangle(point1, point2, point3);
            AddTriangle(point1h, point2h, point3h); // 上面
            //AddTriangle(point1h, point3h, point2h); // 下面
            AddQuad(point1, point2, point2h, point1h);
            AddQuad(point2, point3, point3h, point2h);
            AddQuad(point3, point1, point1h, point3h);
        }

        public void AddQuad(Vector3 lb, Vector3 lt, Vector3 rt, Vector3 rb, float height)
        {
            var h = new Vector3(0, height, 0);
            var lbh = lb + h;
            var lth = lt + h;
            var rth = rt + h;
            var rbh = rb + h;

             //AddQuad(lb, rb, rt, lt);
             AddQuad(lbh, lth, rth, rbh); // 上面
             //AddQuad(lbh, rbh, rth, lth); // 下面
             AddQuad(lb, lbh, rbh, rb);
             AddQuad(rb, rbh, rth, rt);
             AddQuad(rt, rth, lth, lt);
             AddQuad(lt, lth, lbh, lb);
        }

        public void AddPentagon(Vector3 a, Vector3 b, Vector3 c, Vector3 d, Vector3 e, float height)
        {
            var h = new Vector3(0, height, 0);
            var ah = a + h;
            var bh = b + h;
            var ch = c + h;
            var dh = d + h;
            var eh = e + h;
            
            //AddPentagon(a, b, c, d, e);
            AddPentagon(ah, bh, ch, dh, eh); // 上面
            //AddPentagon(ah, eh, dh, ch, bh); // 下面
            AddQuad(a, ah, eh, e);
            AddQuad(b, bh, ah, a);
            AddQuad(c, ch, bh, b);
            AddQuad(d, dh, ch, c);
            AddQuad(e, eh, dh, d);
        }
    }
}