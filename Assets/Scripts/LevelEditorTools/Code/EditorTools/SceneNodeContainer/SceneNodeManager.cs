using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

namespace LevelEditorTools.Nodes
{
    [Flags]
    public enum SceneNodeType
    {
        None = 0x00,
        East = 1 << 0,
        South = 1 << 1,
        West = 1 << 2,
        North = 1 << 3,
        EastSouth = East | South,
        WestSouth = West | South,
        EastNorth = East | North,
        WestNorth = West | North,
        EastWest = East | West,
        SouthNorth = South | North,
        EastWestRoad = 1 << 4,
        SouthNorthRoad = 1 << 5,
    }

    public class SceneNodeManager : MonoBehaviour
    {
        public static Vector3 NomalScale = new Vector3(60, 0, 60);

        public Dictionary<SceneNodeType, Vector3> SceneNodeScale = new Dictionary<SceneNodeType, Vector3>
        {
            {SceneNodeType.None, new Vector3(0, 0, 0)},
            {SceneNodeType.EastWestRoad, new Vector3(40, 0, 20)},
            {SceneNodeType.SouthNorthRoad, new Vector3(20, 0, 40)},
        };

        public int randomSeed = 31;
        public int PointCount = 10;

        public SceneNodeType InitType = SceneNodeType.None;


        public List<GameObject> NodeGoList = new List<GameObject>();

        private List<ScenePointData> _nodeDataList = new List<ScenePointData>();

        public int MaxCount = 10;

        private int curCount = 0;

        public void Create()
        {
            curCount = 0;
            Random.InitState(randomSeed);
            CreateObject(InitType, Vector3.zero, null);
        }

        private void CreateObject(SceneNodeType value, Vector3 pos, ScenePointData parent)
        {
            int index = GetIndex(value);
            GameObject go = Instantiate(NodeGoList[index], pos, Quaternion.identity, transform);
            var data = new ScenePointData
            {
                Type = value,
                CurGameObject = go,
                Position = pos
            };
            data.SetNextLink();
            data.SetLinkData(parent);
            _nodeDataList.Add(data);

            bool canLink = data.CanLinePoint(out List<KeyValuePair<PointType, SceneNodeType>> list);
            if (!canLink)
            {
                return;
            }
            if (curCount >= MaxCount) return;
            curCount++;
            foreach (KeyValuePair<PointType, SceneNodeType> type in list)
            {
                CreateObject(type.Value, GetPosition(data.Type, data.Position, type.Key, type.Value), data);
            }
        }

        private Vector3 GetPosition(SceneNodeType originType, Vector3 originPosition, PointType pointType, SceneNodeType type)
        {
            var targetPosition = Vector3.zero;
            if (!SceneNodeScale.TryGetValue(originType, out Vector3 originScale))
            {
                originScale = NomalScale;
            }

            if (!SceneNodeScale.TryGetValue(type, out Vector3 scale))
            {
                scale = NomalScale;
            }

            switch (pointType)
            {
                case PointType.East:
                    targetPosition = new Vector3(originPosition.x + originScale.x / 2 + scale.x / 2, originPosition.y, originPosition.z);
                    break;
                case PointType.North:
                    targetPosition = new Vector3(originPosition.x, originPosition.y, originPosition.z + originScale.z / 2 + scale.z / 2);
                    break;
                case PointType.South:
                    targetPosition = new Vector3(originPosition.x, originPosition.y, originPosition.z + originScale.z / 2 + scale.z / 2);
                    break;
                case PointType.West:
                    targetPosition = new Vector3(originPosition.x - originScale.x / 2 - scale.x / 2, originPosition.y, originPosition.z);
                    break;
            }


            return targetPosition;
        }

        private int GetIndex(SceneNodeType type)
        {
            switch (type)
            {
                case SceneNodeType.East:
                    return 0;
                case SceneNodeType.South:
                    return 1;
                case SceneNodeType.West:
                    return 2;
                case SceneNodeType.North:
                    return 3;
                case SceneNodeType.EastSouth:
                    return 4;
                case SceneNodeType.WestSouth:
                    return 5;
                case SceneNodeType.EastNorth:
                    return 6;
                case SceneNodeType.WestNorth:
                    return 7;
                case SceneNodeType.EastWest:
                    return 8;
                case SceneNodeType.SouthNorth:
                    return 9;
                case SceneNodeType.EastWestRoad:
                    return 10;
                case SceneNodeType.SouthNorthRoad:
                    return 11;
            }

            return 0;
        }

        public void Destroy()
        {
            _nodeDataList.Clear();
            while (transform.childCount > 0)
            {
                DestroyImmediate(transform.GetChild(0).gameObject);
            }
        }
    }

    public enum PointType
    {
        East = 0x01,
        South = 0x02,
        West = 0x03,
        North = 0x04,
    }

    public class ScenePointData
    {
        private SceneNodeType _type = SceneNodeType.None;

        public SceneNodeType Type
        {
            get => _type;
            set => _type = value;
        }

        public void SetNextLink()
        {
            if ((_type & SceneNodeType.East) > 0)
            {
                LinkPointTypeList[PointType.East] = SceneNodeType.EastWestRoad;
            }

            if ((_type & SceneNodeType.North) > 0)
            {
                LinkPointTypeList[PointType.North] = SceneNodeType.SouthNorthRoad;
            }

            if ((_type & SceneNodeType.West) > 0)
            {
                LinkPointTypeList[PointType.West] = SceneNodeType.EastWestRoad;
            }

            if ((_type & SceneNodeType.South) > 0)
            {
                LinkPointTypeList[PointType.South] = SceneNodeType.SouthNorthRoad;
            }

            // 是道路
            if ((_type & SceneNodeType.EastWestRoad) > 0)
            {
                // 东西路支持两个点
                // East： 支持带有 West
                LinkPointTypeList[PointType.East] = GetRandomWestNode(SceneNodeType.West);
                // West： 支持带有 East
                LinkPointTypeList[PointType.West] = GetRandomWestNode(SceneNodeType.East);
            }
        }

        public void SetLinkData(ScenePointData data)
        {
            if (data == null) return;
            LinkPointList.Add(data);
        }

        private SceneNodeType GetRandomWestNode(SceneNodeType perType)
        {
            List<SceneNodeType> types = new List<SceneNodeType>();
            foreach (var item in Enum.GetValues(typeof(SceneNodeType)))
            {
                var type = (SceneNodeType) item;
                if ((type & perType) > 0)
                {
                    types.Add(type);
                }
            }

            var value = Random.Range(0, types.Count);
            return types[value];
        }

        public Vector3 Position = Vector3.zero;
        public GameObject CurGameObject;

        private Dictionary<PointType, SceneNodeType> LinkPointTypeList = new Dictionary<PointType, SceneNodeType>();
        private List<ScenePointData> LinkPointList = new List<ScenePointData>();

        /// <summary>
        /// 当前点是否支持链接其他点
        /// </summary>
        /// <param name="types">允许链接点的类型</param>
        /// <returns></returns>
        public bool CanLinePoint(out List<KeyValuePair<PointType, SceneNodeType>> types)
        {
            bool canLink = false;
            types = new List<KeyValuePair<PointType, SceneNodeType>>();

            foreach (KeyValuePair<PointType, SceneNodeType> keyValuePair in LinkPointTypeList)
            {
                bool exist = false;
                foreach (ScenePointData pointData in LinkPointList)
                {
                    if (pointData.Type == keyValuePair.Value)
                    {
                        exist = true;
                        break;
                    }
                }

                if (exist) continue;
                canLink = true;
                types.Add(keyValuePair);
            }

            return canLink;
        }
    }
}