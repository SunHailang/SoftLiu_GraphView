using System.Collections.Generic;
using UnityEngine;

public class BezierUtils
{
    /// <summary>
    /// 线性贝赛尔曲线，根据T值，计算贝赛尔曲线上面相对应的点
    /// </summary>
    /// <param name="t">T值</param>
    /// <param name="p0">起点</param>
    /// <param name="p1">终点</param>
    /// <returns></returns>
    private static Vector3 CalculateLineBezierPoint(float t, Vector3 p0, Vector3 p1)
    {
        float u = 1 - t;

        Vector3 p = u * p0;
        p += t * p1;

        return p;
    }

    /// <summary>
    /// 二次贝赛尔曲线，根据T值，计算贝赛尔曲线上面对应的点
    /// </summary>
    /// <param name="t">T值</param>
    /// <param name="p0">起点</param>
    /// <param name="p1">控制点</param>
    /// <param name="p2">终点</param>
    /// <returns></returns>
    private static Vector3 CalculateCubicBezierPoint(float t, Vector3 p0, Vector3 p1, Vector3 p2)
    {
        float u = 1 - t;
        float tt = t * t;
        float uu = u * u;

        Vector3 p = uu * p0;
        p += 2 * u * t * p1;
        p += tt * p2;

        return p;
    }

    /// <summary>
    /// 三次贝赛尔曲线，根据T值，计算贝赛尔曲线上面对应的点
    /// </summary>
    /// <param name="t">插值量</param>
    /// <param name="p0">起点</param>
    /// <param name="p1">控制点1</param>
    /// <param name="p2">控制点2</param>
    /// <param name="p3">终点</param>
    /// <returns></returns>
    private static Vector3 CalculateThreePowerBezierPoint(float t, Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3)
    {
        float u = 1 - t;
        float tt = t * t;
        float uu = u * u;
        float ttt = tt * t;
        float uuu = uu * u;

        Vector3 p = uuu * p0;
        p += 3 * t * uu * p1;
        p += 3 * tt * u * p2;
        p += ttt * p3;

        return p;
    }

    /// <summary>
    /// 获取存储贝赛尔曲线点的数组
    /// </summary>
    /// <param name="startPoint">起点</param>
    /// <param name="endPoint">终点</param>
    /// <param name="segmentNum">采样点数量</param>
    /// <returns></returns>
    public static Vector3[] GetLineBeizerList(Vector3 startPoint, Vector3 endPoint, int segmentNum)
    {
        segmentNum += 2;
        Vector3[] paths = new Vector3[segmentNum];
        paths[0] = startPoint;
        for (int i = 1; i <= segmentNum - 1; i++)
        {
            float t = i / (float) segmentNum;
            Vector3 pixel = CalculateLineBezierPoint(t, startPoint, endPoint);
            paths[i - 1] = pixel;
        }

        paths[segmentNum - 1] = endPoint;
        return paths;
    }

    /// <summary>
    /// 获取存储贝赛尔曲线点的数组
    /// </summary>
    /// <param name="startPoint">起点</param>
    /// <param name="controlPoint">控制点</param>
    /// <param name="endPoint">终点</param>
    /// <param name="segmentNum">采样点数量</param>
    /// <returns></returns>
    public static Vector3[] GetLineBeizerList(Vector3 startPoint, Vector3 controlPoint, Vector3 endPoint, int segmentNum)
    {
        segmentNum += 2;
        Vector3[] paths = new Vector3[segmentNum];
        paths[0] = startPoint;
        for (int i = 2; i <= segmentNum - 1; i++)
        {
            float t = i / (float) segmentNum;
            Vector3 pixel = CalculateCubicBezierPoint(t, startPoint, controlPoint, endPoint);
            paths[i - 1] = pixel;
        }

        paths[segmentNum - 1] = endPoint;
        return paths;
    }

    /// <summary>
    /// 获取存储贝赛尔曲线点的数组
    /// </summary>
    /// <param name="startPoint">起点</param>
    /// <param name="controlPoint1">控制点1</param>
    /// <param name="controlPoint2">控制点2</param>
    /// <param name="endPoint">终点</param>
    /// <param name="segmentNum">采样点数量</param>
    /// <returns></returns>
    public static Vector3[] GetLineBeizerList(Vector3 startPoint, Vector3 controlPoint1, Vector3 controlPoint2, Vector3 endPoint, int segmentNum)
    {
        segmentNum += 2;
        Vector3[] paths = new Vector3[segmentNum];
        paths[0] = startPoint;
        for (int i = 2; i <= segmentNum - 1; i++)
        {
            float t = i / (float) segmentNum;
            Vector3 pixel = CalculateThreePowerBezierPoint(t, startPoint, controlPoint1, controlPoint2, endPoint);
            paths[i - 1] = pixel;
        }

        paths[segmentNum - 1] = endPoint;
        return paths;
    }

    #region n阶曲线，递归实现
    public static Vector3[] GetLineBeizerList(List<Vector3> pointList, int segmentNum)
    {
        List<Vector3> paths = new List<Vector3>(segmentNum);
        int number = pointList.Count;
        if (number <= 2)
        {
            return pointList.ToArray();
        }

        float t = 0f;
        float step = 1 / (float) segmentNum;
        
        do
        {
            Vector3 point = BezierInterpolation(t, pointList, pointList.Count);
            t += step;
            paths.Add(point);
        } while (t <= 1 && segmentNum > 2);
        
        return paths.ToArray();
    }

    private static Vector3 BezierInterpolation(float t, List<Vector3> points, int count)
    {
        Vector3 point = Vector3.zero;
        float[] part = new float[count];
        for (int i = 0; i < count; i++)
        {
            ulong temp = CalcCombinationNumber(count - 1, i);
            point += (temp * points[i] * Mathf.Pow((1 - t), count - 1 - i) * Mathf.Pow(t, i));
        }

        return point;
    }

    private static ulong CalcCombinationNumber(int n, int k)
    {
        ulong[] result = new ulong[n + 1];
        for (int i = 1; i <= n; i++)
        {
            result[i] = 1;
            for (int j = i - 1; j >= 1; j--)
            {
                result[j] += result[j - 1];
            }

            result[0] = 1;
        }

        return result[k];
    }
    #endregion 
}