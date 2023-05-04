using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerQuadTree
{
    public Rectangle boundary;
    public int capacity;
    public Queue<Point> points;

    public QuadTree northeast;
    public QuadTree northwest;
    public QuadTree southeast;
    public QuadTree southwest;
    
    public bool divided;

    public TriggerQuadTree(Rectangle boundary, int n = 4)
    {
        this.boundary = boundary;
        this.capacity = n;
        this.points = new Queue<Point>();
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

    public bool AddPoint(Point point)
    {
        if (!this.boundary.contains(point)) return false;

        if (this.points.Count < this.capacity)
        {
            this.points.Enqueue(point);
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
}