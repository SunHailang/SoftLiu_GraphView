using System.Collections;
using System.Collections.Generic;
using LevelEditorTools.Nodes;
using UnityEditor;
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
        //if (this.boundary.intersects(range))
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
        IEnumerator ie = this.points.GetEnumerator();
        while (ie.MoveNext())
        {
            Point p = (Point) ie.Current;
            if (p != null)
            {
                p.DrawGizmos();
            }
        }
        Gizmos.color = Color.red;
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
    private Transform trans;

    private float m_x;

    public float x
    {
        get
        {
            if (trans != null)
            {
                m_x = trans.position.x;
            }

            return m_x;
        }
    }

    private float m_y;

    public float y
    {
        get
        {
            if (trans != null)
            {
                m_y = trans.position.z;
            }

            return m_y;
        }
    }

    /// <summary>
    /// BoxCollider的大小
    /// </summary>
    public float w = 0;

    public float h = 0;

    public string TriggerGuid = "";
    public int TriggerEventID = 0;
    private TriggerStateEnum m_TriggerState = TriggerStateEnum.None;

    /// <summary>
    /// 支持的触发类型
    /// </summary>
    public void SetTriggerState(TriggerStateEnum state, bool isOnce)
    {
        m_TriggerState = state;
        if (isOnce && (state & TriggerStateEnum.EntryAndExist) > 0)
        {
            triggerCount = (state == TriggerStateEnum.Enter || state == TriggerStateEnum.Exist) ? 1 : 2;
        }
    }

    private int triggerCount = -1;

    public bool CanTrigger(TriggerStateEnum stateEnum)
    {
        if ((stateEnum & m_TriggerState) > 0)
        {
            if (triggerCount < 0) return true;
            if (triggerCount > 0)
            {
                triggerCount--;
                return true;
            }
        }

        return false;
    }


    public Point(Transform _trans, float _w, float _h)
    {
        this.trans = _trans;
        // 初始化为 0
        this.w = _w;
        this.h = _h;
    }

    public Point(float _x, float _y, float _w, float _h)
    {
        m_x = _x;
        m_y = _y;
        // 初始化为 0
        this.w = _w;
        this.h = _h;
    }
#if UNITY_EDITOR
    public void DrawGizmos()
    {
        Gizmos.color = Color.green;
        Vector3 leftDown = new Vector3(x - w / 2, 0, y - h / 2);
        Vector3 leftUp = new Vector3(x - w / 2, 0, y + h / 2);
        Vector3 rightDown = new Vector3(x + w / 2, 0, y - h / 2);
        Vector3 rightUp = new Vector3(x + w / 2, 0, y + h / 2);
        Gizmos.DrawLine(leftDown, leftUp);
        Gizmos.DrawLine(leftDown, rightDown);
        Gizmos.DrawLine(leftUp, rightUp);
        Gizmos.DrawLine(rightDown, rightUp);
        //version 2
        GUIStyle style = new GUIStyle();
        style.normal.textColor = Color.black;
        Handles.Label(new Vector3(x, 0, y), $"{TriggerEventID}", style);
    }
#endif
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
        if (point == null) return false;
        //if (isXZ == 0)
        {
            return ((point.x + point.w / 2) >= (this.x - this.w / 2) &&
                    (point.x - point.w / 2) <= (this.x + this.w / 2) &&
                    (point.y + point.h / 2) >= (this.y - this.h / 2) &&
                    (point.y - point.h / 2) <= (this.y + this.h / 2));
        }
        // else
        // {
        //     return ((point.trans.position.x + point.w / 2) >= (this.x - this.w / 2) &&
        //             (point.trans.position.x - point.w / 2) <= (this.x + this.w / 2) &&
        //             (point.trans.position.y + point.h / 2) >= (this.y - this.h / 2) &&
        //             (point.trans.position.y - point.h / 2) <= (this.y + this.h / 2));
        // }
    }

    public bool intersects(Rectangle range)
    {
        return !(range.x - range.w / 2 > this.x + this.w / 2 ||
                 range.x + range.w / 2 < this.x - this.w / 2 ||
                 range.y - range.h / 2 > this.y + this.h / 2 ||
                 range.y + range.h / 2 < this.y - this.h / 2);
    }

    public bool RectIntersects(Rectangle rect, out Vector2 l, out Vector2 r)
    {
        l = Vector2.zero;
        r = Vector2.zero;

        float Xa1 = x - w / 2;
        float Xa2 = x + w / 2;
        float Ya1 = y - h / 2;
        float Ya2 = y + h / 2;

        float Xb1 = rect.x - rect.w / 2;
        float Xb2 = rect.x + rect.w / 2;
        float Yb1 = rect.y - rect.h / 2;
        float Yb2 = rect.y + rect.h / 2;

        if (Mathf.Abs(Xb2 + Xb1 - Xa2 - Xa1) <= Xa2 - Xa1 + Xb2 - Xb1 &&
            Mathf.Abs(Yb2 + Yb1 - Ya2 - Ya1) <= Ya2 - Ya1 + Yb2 - Yb1)
        {
            float Xc1 = Mathf.Max(Xa1, Xb1);
            float Yc1 = Mathf.Max(Ya1, Yb1);
            l = new Vector2(Xc1, Yc1);
            float Xc2 = Mathf.Min(Xa2, Xb2);
            float Yc2 = Mathf.Min(Ya2, Yb2);
            r = new Vector2(Xc2, Yc2);
            return true;
        }

        return false;
    }

#if UNITY_EDITOR
    public void DrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawLine(leftDown, leftUp);
        Gizmos.DrawLine(leftDown, rightDown);
        Gizmos.DrawLine(leftUp, rightUp);
        Gizmos.DrawLine(rightDown, rightUp);
    }
#endif
}