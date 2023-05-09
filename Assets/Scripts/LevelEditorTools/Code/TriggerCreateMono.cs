using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using LevelEditorTools;
using LevelEditorTools.Nodes;
using LevelEditorTools.Save;
using UnityEngine;

public class TriggerCreateMono : MonoBehaviour
{
    public Transform mainPlayer;
    public Vector3 mainPlayerSize = Vector3.one;
    public Transform EnemyParent = null;
    public GameObject enemyPrefab = null;
    
    private Rectangle mainPlayerRect;

    public LevelTriggerContainer m_triggerContainer = null;

    private List<QuadTree> triggerTreeList;

    private List<GameObject> enemys = new List<GameObject>();

    private void DestroyGo()
    {
        for (int i = transform.childCount - 1; i >= 0; i--)
        {
            DestroyImmediate(transform.GetChild(i).gameObject);
        }
    }
    
    private void Start()
    {
        UpdateTrigger();
    }

    public void UpdateTrigger()
    {
        if (mainPlayer != null)
        {
            // 1. 获取玩家的碰撞框大小
            mainPlayerRect = new Rectangle(mainPlayer.position.x, mainPlayer.position.z, mainPlayerSize.x, mainPlayerSize.z);
        }
        
        DestroyGo();
        triggerTreeList = new List<QuadTree>(m_triggerContainer.LevelDatas.Count);
        foreach (LevelDataScriptable levelData in m_triggerContainer.LevelDatas)
        {
            QuadTree quadTree = new QuadTree(new Rectangle(levelData.LevelPosition.x, levelData.LevelPosition.z, levelData.LevelScale.x, levelData.LevelScale.z), 4);
            HashSet<string> linkList = new HashSet<string>();
            foreach (SceneNodeLinkData sceneNodeLinkData in m_triggerContainer.NodeLinkDatas)
            {
                if (sceneNodeLinkData.InputNodeGuid == levelData.Guid)
                {
                    linkList.Add(sceneNodeLinkData.OutputNodeGuid);
                }
            }

            foreach (BoxTriggerScriptable boxTriggerScriptable in m_triggerContainer.BoxTriggerDatas)
            {
                // 2. 添加可能触发的框体大小 位置
                if (linkList.Contains(boxTriggerScriptable.Guid))
                {
                    GameObject go = new GameObject(boxTriggerScriptable.Guid);
                    go.transform.parent = this.transform;
                    go.transform.localScale = Vector3.one;
                    go.transform.position = boxTriggerScriptable.Position;
                    Point point = new Point(go.transform, boxTriggerScriptable.Scale.x, boxTriggerScriptable.Scale.z);
                    point.TriggerEventID = boxTriggerScriptable.EventID;
                    point.SetTriggerState(boxTriggerScriptable.TriggerState, boxTriggerScriptable.IsOnce);
                    point.EnemyCreatePos = boxTriggerScriptable.EnemyPosition;
                    quadTree.insert(point);
                }
            }

            triggerTreeList.Add(quadTree);
        }
    }
    
    private LinkedList<Point> queryList = new LinkedList<Point>();
    private HashSet<Point> perQueryList = new HashSet<Point>();

    private HashSet<Point> curPoints = new HashSet<Point>();

    private void QueryPointList(QuadTree quadTree)
    {
        queryList.Clear();
        // 判断玩家框是否在 Tree内
        quadTree.query(mainPlayerRect, queryList);
        // 判断上一次在这一次不在 即：退出
        foreach (var point in queryList)
        {
            perQueryList.Remove(point);
        }

        foreach (Point point in perQueryList)
        {
            // exist rect
            if (point.CanTrigger(TriggerStateEnum.Exist))
            {
                Debug.Log($"{point.TriggerEventID}: Exist");
            }

            curPoints.Remove(point);
        }

        perQueryList.Clear();
        if (queryList.Count > 0)
        {
            foreach (Point point in queryList)
            {
                if (curPoints.Contains(point))
                {
                    if (point.CanTrigger(TriggerStateEnum.Stay))
                    {
                        // stay
                        Debug.Log($"{point.TriggerEventID}: Stay");
                    }
                }
                else
                {
                    if (point.CanTrigger(TriggerStateEnum.Enter))
                    {
                        // enter
                        Debug.Log($"{point.TriggerEventID}: Enter, {point.EnemyCreatePos}");
                        CreateEnemy(EnemyParent, point.EnemyCreatePos);
                    }

                    curPoints.Add(point);
                }

                perQueryList.Add(point);
            }
        }
    }

    private void Update()
    {
        if (mainPlayer != null && mainPlayerRect != null)
        {
            // 更新玩家框体的位置
            var position = mainPlayer.position;
            mainPlayerRect.UpdatePosition(position.x, position.z);

            foreach (QuadTree quadTree in triggerTreeList)
            {
                QueryPointList(quadTree);
            }
        }
    }

    private void CreateEnemy(Transform parent, Vector3 pos)
    {
        GameObject go = Instantiate(enemyPrefab, pos, Quaternion.identity, parent);
    }
    
    
#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        if (mainPlayerRect != null)
        {
            mainPlayerRect.DrawGizmos();
        }

        if (triggerTreeList != null && triggerTreeList.Count > 0)
        {
            foreach (QuadTree quadTree in triggerTreeList)
            {
                quadTree.Show();
            }
        }
    }
#endif
}