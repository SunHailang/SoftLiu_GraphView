using System.Collections;
using System.Collections.Generic;
using LevelEditorTools;
using LevelEditorTools.Nodes;
using UnityEditor;
using UnityEngine;
using UnityEngine.Serialization;

public class BezierMono : MonoBehaviour
{
    public string BezierGuidName;
    public SceneContainer SceneContainer;

    public int SegmentNum = 100;
    public float Width = 32;
    public List<Transform> list = new List<Transform>();

    private List<Vector3> points = null;
    private List<Vector3> pointsMax = null;
    private List<Vector3> pointsMin = null;

    public List<QuadRectangle> rectangles;

#if UNITY_EDITOR

    public void LoadPosition()
    {
        if (!string.IsNullOrEmpty(BezierGuidName) && SceneContainer != null)
        {
            list.Clear();
            for (int i = transform.childCount - 1; i >= 0; i--)
            {
                DestroyImmediate(transform.GetChild(i).gameObject);
            }
            
            foreach (SceneBezierScriptable bezierData in SceneContainer.NodeBezierDatas)
            {
                if (bezierData.Guid == BezierGuidName)
                {
                    int index = 0;
                    foreach (Vector3 vector3 in bezierData.ControlPositionList)
                    {
                        GameObject go = new GameObject($"Position_{++index}");
                        go.transform.parent = transform;
                        go.transform.position = vector3;
                        list.Add(go.transform );
                    }

                    SegmentNum = bezierData.SegmentNumber;
                    Width = bezierData.Width;
                }
            }
        }
    }

    public void SetPosition()
    {
        if (list.Count <= 1) return;
        if (!string.IsNullOrEmpty(BezierGuidName) && SceneContainer != null)
        {
            foreach (SceneBezierScriptable bezierData in SceneContainer.NodeBezierDatas)
            {
                if (bezierData.Guid == BezierGuidName)
                {
                    bezierData.ControlPositionList.Clear();
                    foreach (Transform trans in list)
                    {
                        bezierData.ControlPositionList.Add(trans.position);
                    }

                    bezierData.SegmentNumber = SegmentNum;
                    bezierData.Width = Width;
                    bezierData.StartPosition = list[0].position;
                    bezierData.EndPosition = list[^1].position;
                }
            }

            EditorUtility.SetDirty(SceneContainer);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        if (points == null)
        {
            points = new List<Vector3>(list.Count + 2);
        }

        points.Clear();
        foreach (var t in list)
        {
            if (t == null) continue;
            points.Add(t.position);
        }
        if(points.Count <2) return;

        Vector3[] paths = BezierUtils.GetLineBeizerList(points, SegmentNum);
        for (int i = 0; i < paths.Length - 1; i++)
        {
            Gizmos.DrawLine(paths[i], paths[i + 1]);
        }

        // 获取 paths 的最大最小值
        float minX = float.MaxValue;
        float minZ = float.MaxValue;
        float MaxX = float.MinValue;
        float MaxZ = float.MinValue;
        if (pointsMax == null)
        {
            pointsMax = new List<Vector3>(paths.Length);
        }

        pointsMax.Clear();
        if (pointsMin == null)
        {
            pointsMin = new List<Vector3>(paths.Length);
        }

        pointsMin.Clear();
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
        }

        if (rectangles == null) rectangles = new List<QuadRectangle>();
        rectangles.Clear();
        QuadRectangle rect;
        for (int i = 0; i < pointsMin.Count - 1; i++)
        {
            Gizmos.color = Color.magenta;
            Gizmos.DrawLine(pointsMax[i], pointsMax[i + 1]);
            Gizmos.DrawLine(pointsMin[i], pointsMin[i + 1]);

            Gizmos.color = Color.black;
            rect = new QuadRectangle((pointsMin[i].x + pointsMax[i + 1].x) / 2, (pointsMin[i].z + pointsMax[i + 1].z) / 2, Mathf.Abs(pointsMax[i + 1].x - pointsMin[i].x),
                Mathf.Abs(pointsMax[i + 1].z - pointsMin[i].z));
            rect.DrawGizmos();

            Gizmos.color = Color.black;
            rect = new QuadRectangle((pointsMin[i + 1].x + pointsMax[i].x) / 2, (pointsMin[i + 1].z + pointsMax[i].z) / 2, Mathf.Abs(pointsMax[i].x - pointsMin[i + 1].x),
                Mathf.Abs(pointsMax[i].z - pointsMin[i + 1].z));
            rect.DrawGizmos();

            Gizmos.color = Color.white;
            Gizmos.DrawSphere(pointsMax[i], 1);
            Gizmos.DrawSphere(pointsMin[i], 1);
        }

        Gizmos.color = Color.yellow;
        QuadRectangle rectangle = new QuadRectangle((minX + MaxX) / 2, (minZ + MaxZ) / 2, MaxX - minX, MaxZ - minZ);
        rectangle.DrawGizmos();

        Gizmos.color = Color.green;
        Gizmos.DrawSphere(points[0], 2);
        Gizmos.DrawSphere(points[^1], 2);
    }
#endif
}