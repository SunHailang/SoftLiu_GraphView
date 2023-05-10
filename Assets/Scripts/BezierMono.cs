using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BezierMono : MonoBehaviour
{
    public List<Transform> list = new List<Transform>();
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        List<Vector3> points = new List<Vector3>(list.Count + 2);
        points.Add(Vector3.zero);
        for (int i = 0; i < list.Count; i++)
        {
            points.Add(list[i].position);
        }

        points.Add(new Vector3(0, 0, -100));
        Vector3[] paths = BezierUtils.GetLineBeizerList(points, 100);
        for (int i = 0; i < paths.Length - 1; i++)
        {
            Gizmos.DrawLine(paths[i], paths[i + 1]);
        }
    }
}
