using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TriggerTest : MonoBehaviour
{
    public Transform mainPlayer;
    private Rectangle mainPlayerRect;

    public List<Transform> rectList = null;

    private QuadTree triggerTree;

    private void Start()
    {
        // 1. 获取玩家的碰撞框大小
        mainPlayerRect = new Rectangle(mainPlayer.position.x, mainPlayer.position.z, 1, 1);
        // 2. 添加可能触发的框体大小 位置
        triggerTree = new QuadTree(new Rectangle(25, 100, 50, 200), 4);
        foreach (var trans in rectList)
        {
            triggerTree.insert(new Point(trans, 46, 30));
        }
    }

    private LinkedList<Point> queryList = new LinkedList<Point>();
    private HashSet<Point> perQueryList = new HashSet<Point>();

    private HashSet<Point> curPoints = new HashSet<Point>();

    private void Update()
    {
        // 更新玩家框体的位置
        mainPlayerRect.UpdatePosition(mainPlayer.position.x, mainPlayer.position.z);
        
        queryList.Clear();
        // 判断玩家框是否在 Tree内
        triggerTree.query(mainPlayerRect, queryList);
        // 判断上一次在这一次不在 即：退出
        foreach (var point in queryList)
        {
            perQueryList.Remove(point);
        }
        
        foreach (Point point in perQueryList)
        {
            // exist rect
            Debug.Log($"{point.trans.name}: Exist");
            curPoints.Remove(point);
        }
        perQueryList.Clear();
        if (queryList.Count > 0)
        {
            foreach (Point point in queryList)
            {
                if (curPoints.Contains(point))
                {
                    // stay
                    Debug.Log($"{point.trans.name}: Stay");
                }
                else if(!point.IsOnce)
                {
                    // enter
                    Debug.Log($"{point.trans.name}: Enter");
                    curPoints.Add(point);
                    point.IsOnce = true;
                }
                
                perQueryList.Add(point);
            }
        }
    }

    private void OnDrawGizmos()
    {
        if (mainPlayerRect != null)
        {
            mainPlayerRect.DrawGizmos();
        }

        if (triggerTree != null)
        {
            triggerTree.Show();
        }
    }
}