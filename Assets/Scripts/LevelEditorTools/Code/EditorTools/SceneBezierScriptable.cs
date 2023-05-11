using System.Collections.Generic;
using UnityEngine;

namespace LevelEditorTools.Nodes
{
    [System.Serializable]
    public class SceneBezierScriptable : BaseScriptable
    {
        public Vector3 StartPosition;
        public Vector3 EndPosition;

        public float Width;

        public List<Vector3> ControlPositionList = new List<Vector3>();

        public int SegmentNumber = 10;


        public void GetBezierPoint()
        {
        }

        public void CreateGround(Transform parent, Vector3 size, List<GameObject> goList)
        {
            Vector3[] paths = BezierUtils.GetLineBeizerList(ControlPositionList, SegmentNumber);
            // 获取 paths 的最大最小值
            float minX = float.MaxValue;
            float minZ = float.MaxValue;
            float MaxX = float.MinValue;
            float MaxZ = float.MinValue;

            List<Vector3> pointsMax = new List<Vector3>(paths.Length);
            List<Vector3> pointsMin = new List<Vector3>(paths.Length);
            List<QuadRectangle> _rectangles = new List<QuadRectangle>();

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

                if (i == paths.Length - 2)
                {
                    rayMax = new Ray(paths[i + 1], dirMax);
                    max = rayMax.GetPoint(Width / 2);
                    rayMin = new Ray(paths[i + 1], dirMin);
                    min = rayMin.GetPoint(Width / 2);
                    pointsMax.Add(max);
                    pointsMin.Add(min);
                }

                minX = Mathf.Min(min.x, max.x, minX);
                minZ = Mathf.Min(min.z, max.z, minZ);
                MaxX = Mathf.Max(min.x, max.x, MaxX);
                MaxZ = Mathf.Max(min.z, max.z, MaxZ);
                // if (i > 0)
                // {
                //     QuadRectangle rect = new QuadRectangle((_pointsMin[i - 1].x + pointsMax[i].x) / 2, (_pointsMin[i - 1].z + pointsMax[i].z) / 2,
                //         pointsMax[i].x - _pointsMin[i - 1].x,
                //         pointsMax[i].z - _pointsMin[i - 1].z);
                //     _rectangles.Add(rect);
                // }
            }

            for (int i = 0; i < pointsMin.Count - 1; i++)
            {
                QuadRectangle rect = new QuadRectangle((pointsMin[i].x + pointsMax[i + 1].x) / 2, (pointsMin[i].z + pointsMax[i + 1].z) / 2,
                    Mathf.Abs(pointsMax[i + 1].x - pointsMin[i].x), Mathf.Abs(pointsMax[i + 1].z - pointsMin[i].z));
                _rectangles.Add(rect);
                rect = new QuadRectangle((pointsMin[i + 1].x + pointsMax[i].x) / 2, (pointsMin[i + 1].z + pointsMax[i].z) / 2, Mathf.Abs(pointsMax[i].x - pointsMin[i + 1].x),
                    Mathf.Abs(pointsMax[i].z - pointsMin[i + 1].z));
                _rectangles.Add(rect);
            }

            QuadRectangle rectangle = new QuadRectangle((minX + MaxX) / 2, (minZ + MaxZ) / 2, MaxX - minX, MaxZ - minZ);

            int countX = Mathf.RoundToInt(rectangle.w / size.x);
            int countZ = Mathf.RoundToInt(rectangle.h / size.z);
            Vector3 start = new Vector3(rectangle.x - rectangle.w / 2, 0, rectangle.y - rectangle.h / 2);
            for (int i = 0; i < countX; i++)
            {
                for (int j = 0; j < countZ; j++)
                {
                    Vector3 pos = start + new Vector3(i * size.x + size.x / 2, 0, j * size.z + size.z / 2);
                    bool contains = false;
                    foreach (QuadRectangle quadRectangle in _rectangles)
                    {
                        contains = quadRectangle.contains(new Point(pos.x, pos.z, 1, 1));
                        if (contains)
                        {
                            break;
                        }
                    }

                    if (!contains) continue;
                    Object.Instantiate(goList[0], pos, Quaternion.identity, parent);
                }
            }
        }
    }
}