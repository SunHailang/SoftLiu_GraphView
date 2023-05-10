using System.Collections.Generic;
using UnityEngine;

namespace LevelEditorTools.Nodes
{
    [System.Serializable]
    public class GroundScriptable : BaseScriptable
    {
        public int Seed = int.MaxValue;

        // 地板的长宽
        public Vector3 GroundSize = Vector3.one;


        public void CreateGround(SceneScriptable sceneData, Transform parent, List<GameObject> baseList)
        {
            switch (sceneData.SceneType)
            {
                case SceneTypeEnum.Round:
                {
                    float radius = sceneData.Radius;
                    Vector3 centerPos = sceneData.ScenePosition;

                    /* 一个圆分4个象限 */
                    int countX = Mathf.RoundToInt(radius / GroundSize.x);
                    int countZ = Mathf.RoundToInt(radius / GroundSize.z);
                    // 1. 第一象限
                    for (int k = 1; k <= 4; k++)
                    {
                        for (int i = 0; i < countX; i++)
                        {
                            for (int j = 0; j < countZ; j++)
                            {
                                Vector3 pos = GetPos(centerPos, k, i, j);
                                float dis = Vector3.Distance(pos, centerPos);
                                if (dis <= radius)
                                {
                                    Object.Instantiate(baseList[0], pos, Quaternion.identity, parent);
                                }
                            }
                        }
                    }
                }
                    break;
                case SceneTypeEnum.Rectangle:
                {
                    int countX = Mathf.RoundToInt(sceneData.SceneScale.x / GroundSize.x);
                    int countZ = Mathf.RoundToInt(sceneData.SceneScale.z / GroundSize.z);
                    for (int i = 0; i < countX; i++)
                    {
                        for (int j = 0; j < countZ; j++)
                        {
                            Vector3 pos = GetPos(sceneData.ScenePosition, 1, i, j);
                            Object.Instantiate(baseList[0], pos, Quaternion.identity, parent);
                        }
                    }
                }
                    break;
            }
        }

        private Vector3 GetPos(Vector3 start, int index, int x, int z)
        {
            int xData = (index == 1 || index == 4) ? x : -x;
            int zData = (index == 1 || index == 2) ? z : -z;
            float xDelta = (index == 1 || index == 4) ? GroundSize.x / 2 : -GroundSize.x / 2;
            float zDelta = (index == 1 || index == 2) ? GroundSize.z / 2 : -GroundSize.z / 2;
            return start + new Vector3(xData * GroundSize.x + xDelta, 0, zData * GroundSize.z + zDelta);
        }
    }
}