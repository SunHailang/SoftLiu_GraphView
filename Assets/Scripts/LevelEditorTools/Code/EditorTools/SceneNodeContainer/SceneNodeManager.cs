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

        EastSouthNorth = East | South | North,
        WestSouthNorth = West | South | North,
        EastWestSouth = East | West | South,
        EastWestNorth = East | West | North,

        EastWestSouthNorth = East | West | South | North,


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

        private HashSet<ScenePointData> _nodeDataList = new HashSet<ScenePointData>();

        private Dictionary<ScenePointData, QuadRectangle> _SceneRectDict = new Dictionary<ScenePointData, QuadRectangle>();

        public int MaxCount = 10;

        private int curCount = 0;

        public void Create()
        {
            curCount = 0;
            Random.InitState(randomSeed);
            CreateObject(PointType.None, InitType, Vector3.zero, false);
        }

        private ScenePointData CreateObject(PointType point, SceneNodeType value, Vector3 pos, bool close)
        {
            int index = GetIndex(value);
            GameObject go = Instantiate(NodeGoList[index], pos, Quaternion.identity, transform);
            var data = new ScenePointData
            {
                Type = value,
                CurGameObject = go,
                Position = pos
            };

            data.SetLinkData(point);

            data.SetNextLink(close);
            Vector3 scale = GetScale(value);
            QuadRectangle rectangle = new QuadRectangle(pos.x, pos.z, scale.x, scale.z);
            // 判断 新生成的 有没有和之前交叉的如果有交叉则替换 并使用替换的
            foreach (KeyValuePair<ScenePointData, QuadRectangle> valuePair in _SceneRectDict)
            {
                if (valuePair.Value.intersects(rectangle))
                {
                    // 相交 ， 销毁当前 和 Value

                    SceneNodeType newType = GetSceneCommon(data.Type, out bool use1, valuePair.Key.Type, out bool use2);
                    if (use1)
                    {
                        _nodeDataList.Remove(valuePair.Key);
                        DestroyImmediate(valuePair.Key.CurGameObject);
                        foreach (PointType type in valuePair.Key.LinkPointList)
                        {
                            data.SetLinkData(type);
                        }
                    }
                    else if (use2)
                    {
                        DestroyImmediate(go);
                        foreach (PointType type in data.LinkPointList)
                        {
                            valuePair.Key.SetLinkData(type);
                        }

                        data = valuePair.Key;
                    }
                    else
                    {
                        // 新的类型
                        _nodeDataList.Remove(valuePair.Key);
                        DestroyImmediate(valuePair.Key.CurGameObject);
                        DestroyImmediate(go);


                        index = GetIndex(newType);
                        GameObject newGo = Instantiate(NodeGoList[index], pos, Quaternion.identity, transform);
                        ScenePointData newData = new ScenePointData
                        {
                            Type = newType,
                            CurGameObject = newGo,
                            Position = pos
                        };
                        foreach (PointType type in valuePair.Key.LinkPointList)
                        {
                            newData.SetLinkData(type);
                        }

                        foreach (PointType type in data.LinkPointList)
                        {
                            newData.SetLinkData(type);
                        }

                        data = newData;
                    }

                    break;
                }
            }

            _nodeDataList.Add(data);
            _SceneRectDict[data] = rectangle;

            List<KeyValuePair<PointType, SceneNodeType>> tempList = null;
            
            if (curCount >= MaxCount)
            {
                // 将还没有封闭的口  封闭
                ScenePointData closeData = null;
                foreach (ScenePointData pointData in _nodeDataList)
                {
                    bool link = data.CanLinePoint(out tempList);
                    if (link)
                    {
                        closeData = pointData;
                        break;
                    }
                }
                if (closeData == null)
                {
                    return null;
                }
                close = true;
            }
            else
            {
                bool canLink = data.CanLinePoint(out tempList);
                if (!canLink)
                {
                    return null;
                }
            }

            curCount++;
            foreach (KeyValuePair<PointType, SceneNodeType> type in tempList)
            {
                var targetPos = GetPosition(data.Type, data.Position, type.Key, type.Value, out point);
                data.SetLinkData(type.Key);
                var ret = CreateObject(point, type.Value, targetPos, close);
            }

            return data;
        }

        private SceneNodeType GetSceneCommon(SceneNodeType type1, out bool use1, SceneNodeType type2, out bool use2)
        {
            use1 = false;
            use2 = false;

            HashSet<SceneNodeType> types1 = GetNewScene(type1);
            HashSet<SceneNodeType> types2 = GetNewScene(type2);

            HashSet<SceneNodeType> types = new HashSet<SceneNodeType>();
            foreach (SceneNodeType type in types1)
            {
                types.Add(type);
            }

            foreach (SceneNodeType type in types2)
            {
                types.Add(type);
            }

            if (types.Count == types1.Count)
            {
                use1 = true;
                foreach (SceneNodeType type in types1)
                {
                    if (!types.Contains(type))
                    {
                        use1 = false;
                        break;
                    }
                }
            }

            if (use1)
            {
                use2 = false;
                return type1;
            }

            if (types.Count == types2.Count)
            {
                use2 = true;
                foreach (SceneNodeType type in types2)
                {
                    if (!types.Contains(type))
                    {
                        use2 = false;
                        break;
                    }
                }
            }

            if (!use2)
            {
                // 获取新的类型
                SceneNodeType newType = SceneNodeType.None;
                foreach (SceneNodeType type in types)
                {
                    newType |= type;
                }

                return newType;
            }
            else
            {
                return type2;
            }
        }

        private HashSet<SceneNodeType> GetNewScene(SceneNodeType type)
        {
            HashSet<SceneNodeType> types = new HashSet<SceneNodeType>();
            if ((type & SceneNodeType.East) > 0)
            {
                types.Add(SceneNodeType.East);
            }

            if ((type & SceneNodeType.South) > 0)
            {
                types.Add(SceneNodeType.South);
            }

            if ((type & SceneNodeType.West) > 0)
            {
                types.Add(SceneNodeType.West);
            }

            if ((type & SceneNodeType.North) > 0)
            {
                types.Add(SceneNodeType.North);
            }

            return types;
        }

        private Vector3 GetScale(SceneNodeType type)
        {
            if (!SceneNodeScale.TryGetValue(type, out Vector3 scale))
            {
                scale = NomalScale;
            }

            return scale;
        }

        private Vector3 GetPosition(SceneNodeType originType, Vector3 originPosition, PointType originPointType, SceneNodeType type, out PointType targetPointType)
        {
            var targetPosition = Vector3.zero;
            Vector3 originScale = GetScale(originType);
            Vector3 scale = GetScale(type);

            targetPointType = PointType.None;
            switch (originPointType)
            {
                case PointType.East:
                    targetPosition = new Vector3(originPosition.x + originScale.x / 2 + scale.x / 2, originPosition.y, originPosition.z);
                    targetPointType = PointType.West;
                    break;
                case PointType.North:
                    targetPosition = new Vector3(originPosition.x, originPosition.y, originPosition.z + originScale.z / 2 + scale.z / 2);
                    targetPointType = PointType.South;
                    break;
                case PointType.South:
                    targetPosition = new Vector3(originPosition.x, originPosition.y, originPosition.z - originScale.z / 2 - scale.z / 2);
                    targetPointType = PointType.North;
                    break;
                case PointType.West:
                    targetPosition = new Vector3(originPosition.x - originScale.x / 2 - scale.x / 2, originPosition.y, originPosition.z);
                    targetPointType = PointType.East;
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
                case SceneNodeType.EastSouthNorth:
                    return 10;
                case SceneNodeType.WestSouthNorth:
                    return 11;
                case SceneNodeType.EastWestSouth:
                    return 12;
                case SceneNodeType.EastWestNorth:
                    return 13;
                case SceneNodeType.EastWestSouthNorth:
                    return 14;
                case SceneNodeType.EastWestRoad:
                    return 15;
                case SceneNodeType.SouthNorthRoad:
                    return 16;
            }

            return 0;
        }

        public void Destroy()
        {
            _nodeDataList.Clear();
            _SceneRectDict.Clear();
            while (transform.childCount > 0)
            {
                DestroyImmediate(transform.GetChild(0).gameObject);
            }
        }

        private void OnDrawGizmos()
        {
            foreach (KeyValuePair<ScenePointData, QuadRectangle> keyValuePair in _SceneRectDict)
            {
                keyValuePair.Value.DrawGizmos();
            }
        }
    }

    public enum PointType
    {
        None = 0x00,
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

        public void SetNextLink(bool close)
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
                if (!LinkPointList.Contains(PointType.East)) LinkPointTypeList[PointType.East] = GetRandomWestNode(SceneNodeType.West, close);
                // West： 支持带有 East
                if (!LinkPointList.Contains(PointType.West)) LinkPointTypeList[PointType.West] = GetRandomWestNode(SceneNodeType.East, close);
            }

            if ((_type & SceneNodeType.SouthNorthRoad) > 0)
            {
                // South： 支持带有 North
                if (!LinkPointList.Contains(PointType.South)) LinkPointTypeList[PointType.South] = GetRandomWestNode(SceneNodeType.North, close);
                // North： 支持带有 South
                if (!LinkPointList.Contains(PointType.North)) LinkPointTypeList[PointType.North] = GetRandomWestNode(SceneNodeType.South, close);
            }
        }
        
        public void SetLinkData(PointType data)
        {
            if (data == PointType.None) return;
            LinkPointList.Add(data);
        }

        private SceneNodeType GetRandomWestNode(SceneNodeType perType, bool close)
        {
            if (close)
            {
                return perType;
            }
            else
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
        }

        public Vector3 Position = Vector3.zero;
        public GameObject CurGameObject;

        private Dictionary<PointType, SceneNodeType> LinkPointTypeList = new Dictionary<PointType, SceneNodeType>();
        public HashSet<PointType> LinkPointList = new HashSet<PointType>();

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
                if (LinkPointList.Contains(keyValuePair.Key))
                {
                    continue;
                }

                canLink = true;
                types.Add(keyValuePair);
            }

            return canLink;
        }
    }
}