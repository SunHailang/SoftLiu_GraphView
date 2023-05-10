using UnityEngine;

public class QuadCircle
{
    public float Radius = 0;
    
    public Vector3 Position = Vector3.zero;

    public QuadCircle(Vector3 pos, float radius)
    {
        this.Radius = radius;
        this.Position = pos;
    }
    
    public bool IntersectRect(QuadRectangle rect)
    {
        float minRect = Mathf.Min(rect.w, rect.h);

        return false;
    }
    
    
#if UNITY_EDITOR
    public void DrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawSphere(Position, Radius);
    }
#endif
}