using UnityEngine;

public class QuadRectangle
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

    public QuadRectangle(float x, float y, float w, float h)
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

    public bool intersects(QuadRectangle range)
    {
        return !(range.x - range.w / 2 >= this.x + this.w / 2 ||
                 range.x + range.w / 2 <= this.x - this.w / 2 ||
                 range.y - range.h / 2 >= this.y + this.h / 2 ||
                 range.y + range.h / 2 <= this.y - this.h / 2);
    }

    public bool RectIntersects(QuadRectangle rect, out QuadRectangle rectangle)
    {
        bool isIn = this.intersects(rect);
        rectangle = null;
        if (isIn)
        {
            // 当前矩形框 a,  x,y 代表中心坐标， w,h 代表 宽、高
            Vector2 aLeftUp = new Vector2(x - w / 2, y + h / 2);
            Vector2 aRightBottom = new Vector2(x + w / 2, y - h / 2);

            // rect矩形 b, 
            Vector2 bLeftUp = new Vector2(rect.x - rect.w / 2, rect.y + rect.h / 2);
            Vector2 bRightBottom = new Vector2(rect.x + rect.w / 2, rect.y - rect.h / 2);
            // 只考虑相交，
            Vector2 inLeftUp = new Vector2(Mathf.Max(aLeftUp.x, bLeftUp.x), Mathf.Min(aLeftUp.y, bLeftUp.y));
            Vector2 inRightBottom = new Vector2(Mathf.Min(aRightBottom.x, bRightBottom.x), Mathf.Max(aRightBottom.y, bRightBottom.y));
            float inX = (inLeftUp.x + inRightBottom.x) / 2;
            float inY = (inLeftUp.y + inRightBottom.y) / 2;
            float inW = inRightBottom.x - inLeftUp.x;
            float inH = inLeftUp.y - inRightBottom.y;
            rectangle = new QuadRectangle(inX, inY, inW, inH);
        }

        return isIn;
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