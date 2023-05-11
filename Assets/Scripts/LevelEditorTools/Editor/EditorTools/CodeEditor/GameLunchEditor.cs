using System.Collections.Generic;
using LevelEditorTools;
using LevelEditorTools.Editor.Nodes;
using LevelEditorTools.Nodes;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(GameDemoLunch), true)]
public class GameLunchEditor : UnityEditor.Editor
{
    private SceneContainer _sceneContainer;

    private GameObject _selectTarget;

    private GameDemoLunch _lunch;

    private void OnEnable()
    {
        _selectTarget = Selection.activeObject as GameObject;
        if (_selectTarget != null)
        {
            _lunch = _selectTarget.GetComponent<GameDemoLunch>();
            if (_lunch != null)
            {
                _sceneContainer = _lunch.m_SceneContainer;
            }
        }
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        if (_lunch != null)
        {
            _sceneContainer = _lunch.m_SceneContainer;
        }

        if (_sceneContainer == null) return;
        serializedObject.Update();
        if (GUILayout.Button("Create Scene", GUI.skin.button, GUILayout.Height(25)))
        {
            CreateScene();
        }

        if (GUILayout.Button("Destroy Scene", GUI.skin.button, GUILayout.Height(25)))
        {
            DestroyScene();
        }

        serializedObject.ApplyModifiedProperties();
    }

    private Vector3 _curSceneScale = Vector3.one;
    private Vector3 _curScenePosition = Vector3.zero;

    private void CreateScene()
    {
        if (_sceneContainer == null || _selectTarget == null)
        {
            return;
        }

        DestroyScene();
        Transform parent = _selectTarget.transform;
        for (int i = 0; i < _sceneContainer.NodeSceneDatas.Count; i++)
        {
            // 获取任意房间之间墙体重叠的位置
            for (int j = i + 1; j < _sceneContainer.NodeSceneDatas.Count; j++)
            {
                SceneScriptable leftNode = _sceneContainer.NodeSceneDatas[i];
                if (leftNode.SceneType == SceneTypeEnum.Round) continue;
                Vector3 leftPos = new Vector3(leftNode.ScenePosition.x + leftNode.SceneScale.x / 2, leftNode.ScenePosition.y, leftNode.ScenePosition.z + leftNode.SceneScale.z / 2);
                QuadRectangle leftRect = new QuadRectangle(leftPos.x, leftPos.z, leftNode.SceneScale.x, leftNode.SceneScale.z);
                SceneScriptable rightNode = _sceneContainer.NodeSceneDatas[j];
                if (rightNode.SceneType == SceneTypeEnum.Round) continue;
                Vector3 rightPos = new Vector3(rightNode.ScenePosition.x + rightNode.SceneScale.x / 2, rightNode.ScenePosition.y,
                    rightNode.ScenePosition.z + rightNode.SceneScale.z / 2);
                QuadRectangle rightRect = new QuadRectangle(rightPos.x, rightPos.z, rightNode.SceneScale.x, rightNode.SceneScale.z);

                if (leftRect.RectIntersects(rightRect, out QuadRectangle rect))
                {
                    _lunch.sceneNodeDatas.Add(rect);
                }
            }
        }

        for (int i = 0; i < _sceneContainer.NodeBezierDatas.Count; i++)
        {
            SceneBezierScriptable data = _sceneContainer.NodeBezierDatas[i];
            // 1. 创建Scene节点
            GameObject go = new GameObject(data.Title);
            go.transform.parent = parent;
            go.transform.position = data.StartPosition;
            // 2. 查找这个 Scene 的子节点
            List<BaseScriptable> list = GetSceneChild(data);
            CreateGroundFramework(data, go.transform, list);
        }
        
        for (int i = 0; i < _sceneContainer.NodeSceneDatas.Count; i++)
        {
            SceneScriptable data = _sceneContainer.NodeSceneDatas[i];
            // 1. 创建Scene节点
            GameObject go = new GameObject(data.Title);
            go.transform.parent = parent;
            go.transform.position = data.ScenePosition;
            // 2. 查找这个 Scene 的子节点
            List<BaseScriptable> list = GetSceneChild(data);
            _curSceneScale = data.SceneScale;
            _curScenePosition = data.ScenePosition;
            CreateGroundFramework(data, go.transform, list);
        }
    }

    private void CreateGroundFramework(SceneBezierScriptable sceneData, Transform parent, List<BaseScriptable> baseList)
    {
        foreach (BaseScriptable scriptable in baseList)
        {
            if (scriptable is GroundScriptable groundScriptable)
            {
                var go = new GameObject(groundScriptable.Title)
                {
                    transform =
                    {
                        parent = parent
                    }
                };
                List<GameObjectScriptable> list = GetGoScriptable(groundScriptable);
                if (!GetGoList(list, out List<KeyValuePair<float, float>> randomList, out List<GameObject> goList))
                {
                    Debug.LogError($"CreateGroundGo Error:");
                    return;
                }

                sceneData.CreateGround(go.transform, groundScriptable.GroundSize, goList);
            }
        }
    }

    private List<BaseScriptable> GetSceneChild(BaseScriptable scene)
    {
        List<BaseScriptable> list = new List<BaseScriptable>();

        foreach (SceneNodeLinkData linkData in _sceneContainer.NodeLinkDatas)
        {
            if (linkData.OutputNodeGuid == scene.Guid)
            {
                foreach (var groundData in _sceneContainer.NodeGroundDatas)
                {
                    if (groundData.Guid == linkData.InputNodeGuid)
                    {
                        list.Add(groundData);
                    }
                }

                foreach (DoorScriptable doorData in _sceneContainer.NodeDoorDatas)
                {
                    if (doorData.Guid == linkData.InputNodeGuid)
                    {
                        list.Add(doorData);
                    }
                }

                foreach (WallScriptable wallData in _sceneContainer.NodeWallDatas)
                {
                    if (wallData.Guid == linkData.InputNodeGuid)
                    {
                        list.Add(wallData);
                    }
                }

                foreach (ObstacleScriptable obstacleData in _sceneContainer.NodeObstacleDatas)
                {
                    if (obstacleData.Guid == linkData.InputNodeGuid)
                    {
                        list.Add(obstacleData);
                    }
                }
            }
        }

        return list;
    }

    private List<GameObjectScriptable> GetGoScriptable(BaseScriptable scriptable)
    {
        List<GameObjectScriptable> list = new List<GameObjectScriptable>();
        foreach (SceneNodeLinkData linkData in _sceneContainer.NodeLinkDatas)
        {
            if (linkData.OutputNodeGuid == scriptable.Guid)
            {
                foreach (GameObjectScriptable goData in _sceneContainer.NodeGameObjectDatas)
                {
                    if (goData.Guid == linkData.InputNodeGuid)
                    {
                        list.Add(goData);
                    }
                }
            }
        }

        return list;
    }

    private void CreateGroundFramework(SceneScriptable sceneData, Transform parent, List<BaseScriptable> baseList)
    {
        foreach (BaseScriptable scriptable in baseList)
        {
            if (scriptable is DoorScriptable doorScriptable)
            {
                var go = new GameObject(doorScriptable.Title)
                {
                    transform =
                    {
                        parent = parent
                    }
                };
                CreateDoorGo(go.transform, doorScriptable.DoorSize, GetGoScriptable(doorScriptable));
            }
        }

        foreach (BaseScriptable scriptable in baseList)
        {
            if (scriptable is GroundScriptable groundScriptable)
            {
                var go = new GameObject(groundScriptable.Title)
                {
                    transform =
                    {
                        parent = parent
                    }
                };
                List<GameObjectScriptable> list = GetGoScriptable(groundScriptable);
                if (!GetGoList(list, out List<KeyValuePair<float, float>> randomList, out List<GameObject> goList))
                {
                    Debug.LogError($"CreateGroundGo Error:");
                    return;
                }

                groundScriptable.CreateGround(sceneData, go.transform, goList);
                //CreateGroundGo(go.transform, groundScriptable.Seed, groundScriptable.GroundSize, list);
            }
            else if (scriptable is WallScriptable wallScriptable)
            {
                var go = new GameObject(wallScriptable.Title)
                {
                    transform =
                    {
                        parent = parent
                    }
                };
                CreateWallGo(go.transform, wallScriptable.Seed, wallScriptable.WallSize, GetGoScriptable(wallScriptable));
            }
            else if (scriptable is ObstacleScriptable obstacleScriptable)
            {
                GameObject go = new GameObject(obstacleScriptable.Title)
                {
                    transform =
                    {
                        parent = parent
                    }
                };
                List<GameObjectScriptable> list = GetGoScriptable(obstacleScriptable);
                CreateObstacleGo(go.transform, obstacleScriptable.Seed, list);
            }
        }
    }

    private void CreateGroundGo(Transform parent, int seed, Vector3 size, List<GameObjectScriptable> baseList)
    {
        int xCount = (int) (_curSceneScale.x / size.x);
        int zCount = (int) (_curSceneScale.z / size.z);

        if (!GetGoList(baseList, out List<KeyValuePair<float, float>> list, out List<GameObject> goList))
        {
            Debug.LogError($"CreateGroundGo Error:");
            return;
        }

        Random.InitState(seed);
        for (int i = 0; i < xCount; i++)
        {
            for (int j = 0; j < zCount; j++)
            {
                var value = Random.Range(0f, 1.0f);
                if (!GetRandomIndex(value, list, out var index))
                {
                    continue;
                }

                GameObject go = Instantiate(goList[index], parent);
                go.name = goList[index].name;
                go.transform.position = GetTransPosition(new Vector3(i * size.x, 0, j * size.z));
                go.transform.localScale = baseList[index].Scale;
                go.isStatic = baseList[index].ForceStatic;
            }
        }
    }

    private Vector3 GetTransPosition(Vector3 basePos)
    {
        return _curScenePosition + basePos;
    }

    private bool GetGoList(List<GameObjectScriptable> baseList, out List<KeyValuePair<float, float>> list, out List<GameObject> goList)
    {
        float data = 0;
        list = new List<KeyValuePair<float, float>>(baseList.Count);
        goList = new List<GameObject>(baseList.Count);
        for (int i = 0; i < baseList.Count; i++)
        {
            GameObjectScriptable goScriptable = baseList[i];
            KeyValuePair<float, float> item = new KeyValuePair<float, float>(data, data + goScriptable.Probability);
            data += goScriptable.Probability;
            list.Add(item);
            goList.Add(AssetDatabase.LoadAssetAtPath<GameObject>(AssetDatabase.GUIDToAssetPath(goScriptable.TemplateGo)));
        }

        return goList.Count > 0;
    }

    private void CreateDoorGo(Transform parent, Vector3 size, List<GameObjectScriptable> baseList)
    {
        if (!GetGoList(baseList, out List<KeyValuePair<float, float>> list, out List<GameObject> goList))
        {
            Debug.LogError($"CreateDoorGo Error:");
            return;
        }

        int index = 0;
        Vector3 pos = baseList[index].Position;
        var go = Instantiate(goList[index], parent);
        go.name = goList[index].name;
        go.transform.position = pos;
        go.transform.localScale = baseList[index].Scale;
        go.transform.Rotate(Vector3.up, baseList[index].Rotation.x);
        go.isStatic = baseList[index].ForceStatic;

        _lunch.sceneDoorDatas.Add(new QuadRectangle(pos.x, pos.z, size.x, size.z));
    }

    private void CreateWallGo(Transform parent, int seed, Vector3 size, List<GameObjectScriptable> baseList)
    {
        int xCount = (int) (_curSceneScale.x / size.x);
        int zCount = (int) (_curSceneScale.z / size.z);

        if (!GetGoList(baseList, out List<KeyValuePair<float, float>> list, out List<GameObject> goList))
        {
            Debug.LogError($"CreateGroundGo Error:");
            return;
        }

        List<float> xList = new List<float>() {0, _curSceneScale.x - size.x};
        List<float> zList = new List<float>() {0, _curSceneScale.z - size.z};
        UnityEngine.Random.InitState(seed);

        HashSet<QuadRectangle> rectIndexs = new HashSet<QuadRectangle>();

        for (int i = 0; i < xCount; i++)
        {
            for (int j = 0; j < zList.Count; j++)
            {
                Vector3 pos = GetTransPosition(new Vector3(i * size.x, 0, zList[j]));
                Point point = new Point(pos.x, pos.z, 1, 1);
                bool isContinue = false;
                foreach (QuadRectangle doorData in _lunch.sceneDoorDatas)
                {
                    if (doorData.contains(point))
                    {
                        isContinue = true;
                        break;
                    }
                }
                if (isContinue) continue;

                for (int k = 0; k < _lunch.sceneNodeDatas.Count; k++)
                {
                    QuadRectangle rectangle = _lunch.sceneNodeDatas[k];
                    if (rectangle.contains(point))
                    {
                        isContinue = true;
                        rectIndexs.Add(rectangle);
                        break;
                    }
                }

                if (isContinue) continue;

                float value = UnityEngine.Random.Range(0f, 1.0f);
                if (!GetRandomIndex(value, list, out int index))
                {
                    continue;
                }

                var go = Instantiate(goList[index], parent);
                go.name = goList[index].name;
                go.transform.position = GetTransPosition(new Vector3(i * size.x + 2, 0, zList[j] + 2));
                go.transform.localScale = baseList[index].Scale;
                go.transform.Rotate(Vector3.up, baseList[index].Rotation.x);
                go.isStatic = baseList[index].ForceStatic;
            }
        }

        for (int i = 0; i < zCount; i++)
        {
            for (int j = 0; j < xList.Count; j++)
            {
                Vector3 pos = GetTransPosition(new Vector3(xList[j] + 2, 0, i * size.z + 2));
                Point point = new Point(pos.x, pos.z, 1, 1);
                bool isContinue = false;
                foreach (QuadRectangle doorData in _lunch.sceneDoorDatas)
                {
                    if (doorData.contains(point))
                    {
                        isContinue = true;
                        break;
                    }
                }
                if (isContinue) continue;

                for (int k = 0; k < _lunch.sceneNodeDatas.Count; k++)
                {
                    QuadRectangle rectangle = _lunch.sceneNodeDatas[k];
                    if (rectangle.contains(point))
                    {
                        isContinue = true;
                        rectIndexs.Add(rectangle);
                        break;
                    }
                }

                if (isContinue) continue;

                float value = UnityEngine.Random.Range(0f, 1.0f);
                if (!GetRandomIndex(value, list, out int index))
                {
                    continue;
                }

                GameObject go = Instantiate<GameObject>(goList[index], parent);
                go.name = goList[index].name;
                go.transform.position = pos;
                go.transform.localScale = baseList[index].Scale;
                go.transform.Rotate(Vector3.up, baseList[index].Rotation.z);
                go.isStatic = baseList[index].ForceStatic;
            }
        }

        // foreach (QuadRectangle i in rectIndexs)
        // {
        //     _lunch.sceneNodeDatas.Remove(i);
        // }
    }

    private void CreateObstacleGo(Transform parent, int seed, List<GameObjectScriptable> baseList)
    {
        float widthStep = 2;
        int xCount = (int) (_curSceneScale.x / widthStep);
        int zCount = (int) (_curSceneScale.z / widthStep);

        if (!GetGoList(baseList, out List<KeyValuePair<float, float>> list, out List<GameObject> goList))
        {
            Debug.LogError($"CreateGroundGo Error:");
            return;
        }

        Random.InitState(seed);
        for (int i = 2; i < xCount - 2; i++)
        {
            for (int j = 2; j < zCount - 2; j++)
            {
                float value = Random.Range(0f, 1.0f);
                if (!GetRandomIndex(value, list, out int index))
                {
                    continue;
                }

                GameObject go = Instantiate<GameObject>(goList[index], parent);
                go.name = goList[index].name;
                go.transform.position = GetTransPosition(new Vector3(i * widthStep, 0, j * widthStep));
                go.transform.localScale = baseList[index].Scale;
                go.transform.Rotate(Vector3.up, baseList[index].Rotation.y);
                go.isStatic = baseList[index].ForceStatic;
            }
        }
    }

    private bool GetRandomIndex(float value, List<KeyValuePair<float, float>> list, out int index)
    {
        index = -1;
        for (int i = 0; i < list.Count; i++)
        {
            if (value >= list[i].Key && value < list[i].Value)
            {
                index = i;
                break;
            }
        }

        return index >= 0;
    }

    private void DestroyScene()
    {
        if (_selectTarget == null || _lunch == null) return;
        _lunch.sceneDoorDatas.Clear();
        _lunch.sceneNodeDatas.Clear();
        var childCount = _selectTarget.transform.childCount;
        for (var i = childCount - 1; i >= 0; i--)
        {
            DestroyImmediate(_selectTarget.transform.GetChild(i).gameObject);
        }
    }
}