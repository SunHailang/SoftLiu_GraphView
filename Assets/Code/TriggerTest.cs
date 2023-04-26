using System;
using System.Collections;
using System.Collections.Generic;
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
        for (int i = 0; i < rectList.Count; i++)
        {
            Transform trans = rectList[i];
            triggerTree.insert(new Point(trans, 46, 30));
        }
    }

    private void FixedUpdate()
    {
        // 更新玩家框体的位置
        mainPlayerRect.UpdatePosition(mainPlayer.position.x, mainPlayer.position.z);
    }

    private Queue<Point> queryList = new Queue<Point>();
    private void Update()
    {
        queryList.Clear();
        // 判断玩家框是否在 Tree内
        triggerTree.query(mainPlayerRect, queryList);
        if (queryList.Count > 0)
        {
            foreach (Point point in queryList)
            {
                Debug.Log($"{point.trans.name}");
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