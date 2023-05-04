using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class QuadTree
{
    public Rectangle boundary;
    public int capacity;
    public LinkedList<Point> points;

    public QuadTree northeast;
    public QuadTree northwest;
    public QuadTree southeast;
    public QuadTree southwest;

    public bool divided;

    public QuadTree(Rectangle boundary, int n)
    {
        this.boundary = boundary;
        this.capacity = n;
        this.points = new LinkedList<Point>();
        this.divided = false;
    }

    private void subdivde()
    {
        float x = this.boundary.x;
        float y = this.boundary.y;
        float w = this.boundary.w;
        float h = this.boundary.h;
        Rectangle ne = new Rectangle(x + w / 4, y + h / 4, w / 2, h / 2);
        this.northeast = new QuadTree(ne, this.capacity);
        Rectangle nw = new Rectangle(x - w / 4, y + h / 4, w / 2, h / 2);
        this.northwest = new QuadTree(nw, this.capacity);
        Rectangle se = new Rectangle(x + w / 4, y - h / 4, w / 2, h / 2);
        this.southeast = new QuadTree(se, this.capacity);
        Rectangle sw = new Rectangle(x - w / 4, y - h / 4, w / 2, h / 2);
        this.southwest = new QuadTree(sw, this.capacity);
        this.divided = true;
    }

    public bool insert(Point point)
    {
        if (!this.boundary.contains(point)) return false;

        if (this.points.Count < this.capacity)
        {
            this.points.AddLast(point);
            return true;
        }

        if (!divided)
        {
            this.subdivde();
        }

        if (this.northeast.insert(point)) return true;
        if (this.northwest.insert(point)) return true;
        if (this.southeast.insert(point)) return true;
        if (this.southwest.insert(point)) return true;

        return false;
    }

    public bool remove(Point point)
    {
        bool isExist = false;
        IEnumerator ie = this.points.GetEnumerator();
        while (ie.MoveNext())
        {
            Point p = ie.Current as Point;
            if (p == point)
            {
                isExist = true;
                break;
            }
        }

        if (isExist)
        {
            this.points.Remove(point);
            return true;
        }

        if (this.divided)
        {
            if (this.northwest.remove(point))
            {
                return true;
            }

            if (this.northeast.remove(point))
            {
                return true;
            }

            if (this.southwest.remove(point))
            {
                return true;
            }

            if (this.southeast.remove(point))
            {
                return true;
            }
        }

        return false;
    }

    public void query(Rectangle range, LinkedList<Point> found = null)
    {
        if (found == null) found = new LinkedList<Point>();
        if (this.boundary.intersects(range))
        {
            IEnumerator ie = this.points.GetEnumerator();
            while (ie.MoveNext())
            {
                if (ie.Current is Point p && range.contains(p))
                {
                    found.AddLast(p);
                }
            }

            if (!this.divided) return;
            this.northwest.query(range, found);
            this.northeast.query(range, found);
            this.southwest.query(range, found);
            this.southeast.query(range, found);
        }
    }


#if UNITY_EDITOR
    public void Show()
    {
        Gizmos.color = Color.red;
        IEnumerator ie = this.points.GetEnumerator();
        while (ie.MoveNext())
        {
            Point p = (Point) ie.Current;
            if (p != null)
            {
                p.DrawGizmos();
            }
        }

        Gizmos.DrawLine(this.boundary.leftDown, this.boundary.leftUp);
        Gizmos.DrawLine(this.boundary.leftDown, this.boundary.rightDown);
        Gizmos.DrawLine(this.boundary.leftUp, this.boundary.rightUp);
        Gizmos.DrawLine(this.boundary.rightDown, this.boundary.rightUp);
        if (this.divided)
        {
            this.northeast.Show();
            this.northwest.Show();
            this.southeast.Show();
            this.southwest.Show();
        }
    }
#endif
}

/// <summary>
/// 假设点是带有碰撞盒的
/// </summary>
public class Point
{
    public Transform trans;

    /// <summary>
    /// BoxCollider的大小
    /// </summary>
    public float w = 0;

    public float h = 0;

    public bool IsOnce = false;

    public Point(Transform _trans, float _w, float _h)
    {
        this.trans = _trans;
        // 初始化为 0
        this.w = _w;
        this.h = _h;
    }
    public void DrawGizmos()
    {
        Gizmos.color = Color.green;
        float x = trans.position.x;
        float y = trans.position.z;
        Vector3 leftDown = new Vector3(x - w / 2, 0, y - h / 2);
        Vector3 leftUp = new Vector3(x - w / 2, 0, y + h / 2);
        Vector3 rightDown = new Vector3(x + w / 2, 0, y - h / 2);
        Vector3 rightUp = new Vector3(x + w / 2, 0, y + h / 2);
        Gizmos.DrawLine(leftDown, leftUp);
        Gizmos.DrawLine(leftDown, rightDown);
        Gizmos.DrawLine(leftUp, rightUp);
        Gizmos.DrawLine(rightDown, rightUp);
    }
}

public class Rectangle
{
    public float x;
    public float y;
    public float w;
    public float h;

    // 得到4个顶点的坐标
    public Vector3 leftDown;
    public Vector3 leftUp;
    public Vector3 rightDown;
    public Vector3 rightUp;

    public Rectangle(float x, float y, float w, float h)
    {
        this.w = w;
        this.h = h;

        this.x = x;
        this.y = y;
        this.leftDown = new Vector3(this.x - w / 2, 0, this.y - h / 2);
        this.leftUp = new Vector3(this.x - w / 2, 0, this.y + h / 2);
        this.rightDown = new Vector3(this.x + w / 2, 0, this.y - h / 2);
        this.rightUp = new Vector3(this.x + w / 2, 0, this.y + h / 2);
    }

    public void UpdatePosition(float x, float y)
    {
        this.x = x;
        this.y = y;
        this.leftDown = new Vector3(this.x - w / 2, 0, this.y - h / 2);
        this.leftUp = new Vector3(this.x - w / 2, 0, this.y + h / 2);
        this.rightDown = new Vector3(this.x + w / 2, 0, this.y - h / 2);
        this.rightUp = new Vector3(this.x + w / 2, 0, this.y + h / 2);
    }
    
    /// <summary>
    /// 带BoxCollider的大小  假设 BoxCollider的中心点和物体本身重合
    /// </summary>
    /// <param name="point"></param>
    /// <returns></returns>
    public bool contains(Point point, int isXZ = 0)
    {
        if (point == null || point.trans == null) return false;
        if (isXZ == 0)
        {
            return ((point.trans.position.x + point.w / 2) >= (this.x - this.w / 2) &&
                    (point.trans.position.x - point.w / 2) <= (this.x + this.w / 2) &&
                    (point.trans.position.z + point.h / 2) >= (this.y - this.h / 2) &&
                    (point.trans.position.z - point.h / 2) <= (this.y + this.h / 2));
        }
        else
        {
            return ((point.trans.position.x + point.w / 2) >= (this.x - this.w / 2) &&
                    (point.trans.position.x - point.w / 2) <= (this.x + this.w / 2) &&
                    (point.trans.position.y + point.h / 2) >= (this.y - this.h / 2) &&
                    (point.trans.position.y - point.h / 2) <= (this.y + this.h / 2));
        }
    }

    public bool intersects(Rectangle range)
    {
        return !(range.x - range.w / 2 > this.x + this.w / 2 ||
                 range.x + range.w / 2 < this.x - this.w / 2 ||
                 range.y - range.h / 2 > this.y + this.h / 2 ||
                 range.y + range.h / 2 < this.y - this.h / 2);
    }


    public void DrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawLine(leftDown, leftUp);
        Gizmos.DrawLine(leftDown, rightDown);
        Gizmos.DrawLine(leftUp, rightUp);
        Gizmos.DrawLine(rightDown, rightUp);
    }
}