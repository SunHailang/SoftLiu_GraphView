using System.Collections.Generic;
using LevelEditorTools;
using LevelEditorTools.Nodes;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(GameDemoLunch), true)]
public class GameLunchEditor : UnityEditor.Editor
{
    private SceneContainer _sceneContainer;

    private GameObject _selectTarget;

    private void OnEnable()
    {
        _selectTarget = Selection.activeObject as GameObject;

        string guid = EditorPrefs.GetString("SceneContainer");
        if (!string.IsNullOrEmpty(guid))
        {
            _sceneContainer = AssetDatabase.LoadAssetAtPath<SceneContainer>(AssetDatabase.GUIDToAssetPath(guid));
            instanceID = _sceneContainer.GetInstanceID();
        }
    }

    private int instanceID = 0;

    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        _sceneContainer = EditorGUILayout.ObjectField("SceneContainer", _sceneContainer, typeof(SceneContainer), false) as SceneContainer;
        int newId = _sceneContainer == null ? 0 : _sceneContainer.GetInstanceID();
        if (instanceID != newId)
        {
            instanceID = newId;
            string guid = "";
            if (instanceID != 0 &&_sceneContainer != null)
            {
                string path = AssetDatabase.GetAssetPath(_sceneContainer.GetInstanceID());
                guid = AssetDatabase.AssetPathToGUID(path);
            }
            EditorPrefs.SetString("SceneContainer", guid);
        }

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

        foreach (SceneScriptable data in _sceneContainer.NodeSceneDatas)
        {
            // 1. 创建Scene节点
            GameObject go = new GameObject(data.Title);
            go.transform.parent = parent;
            go.transform.position = data.ScenePosition;
            // 2. 查找这个 Scene 的子节点
            List<BaseScriptable> list = GetSceneChild(data);
            _curSceneScale = data.SceneScale;
            _curScenePosition = data.ScenePosition;
            CreateGroundFramework(go.transform, list);
        }
    }

    private List<BaseScriptable> GetSceneChild(SceneScriptable scene)
    {
        List<BaseScriptable> list = new List<BaseScriptable>();
        foreach (SceneNodeLinkData linkData in _sceneContainer.NodeLinkDatas)
        {
            if (linkData.OutputNodeGuid == scene.Guid)
            {
                foreach (GroundScriptable groundData in _sceneContainer.NodeGroundDatas)
                {
                    if (groundData.Guid == linkData.InputNodeGuid)
                    {
                        list.Add(groundData);
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

    private void CreateGroundFramework(Transform parent, List<BaseScriptable> baseList)
    {
        foreach (BaseScriptable scriptable in baseList)
        {
            if (scriptable is GroundScriptable groundScriptable)
            {
                GameObject go = new GameObject(groundScriptable.Title);
                go.transform.parent = parent;
                List<GameObjectScriptable> list = GetGoScriptable(groundScriptable);
                CreateGroundGo(go.transform, groundScriptable.Seed, list);
            }
            else if (scriptable is WallScriptable wallScriptable)
            {
                GameObject go = new GameObject(wallScriptable.Title);
                go.transform.parent = parent;
                List<GameObjectScriptable> list = GetGoScriptable(wallScriptable);
                CreateWallGo(go.transform, wallScriptable.Seed, list);
            }
            else if (scriptable is ObstacleScriptable obstacleScriptable)
            {
                GameObject go = new GameObject(obstacleScriptable.Title);
                go.transform.parent = parent;
                List<GameObjectScriptable> list = GetGoScriptable(obstacleScriptable);
                CreateObstacleGo(go.transform, obstacleScriptable.Seed, list);
            }
        }
    }

    private void CreateGroundGo(Transform parent, int seed, List<GameObjectScriptable> baseList)
    {
        float widthStep = 4;
        float lenStep = 4;

        int xCount = (int) (_curSceneScale.x / widthStep);
        int zCount = (int) (_curSceneScale.z / lenStep);

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
                float value = UnityEngine.Random.Range(0f, 1.0f);
                if (!GetRandomIndex(value, list, out int index))
                {
                    continue;
                }
                GameObject go = Instantiate<GameObject>(goList[index], parent);
                go.transform.position =GetTransPosition(new Vector3(i * widthStep, 0, j * lenStep));
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

    private void CreateWallGo(Transform parent, int seed, List<GameObjectScriptable> baseList)
    {
        float lenStep = 4;
        int xCount = (int) (_curSceneScale.x / lenStep);
        int zCount = (int) (_curSceneScale.z / lenStep);

        if (!GetGoList(baseList, out List<KeyValuePair<float, float>> list, out List<GameObject> goList))
        {
            Debug.LogError($"CreateGroundGo Error:");
            return;
        }

        List<float> xList = new List<float>() {0, _curSceneScale.x - lenStep};
        List<float> zList = new List<float>() {0, _curSceneScale.z - lenStep};
        UnityEngine.Random.InitState(seed);
        for (int i = 0; i < xCount; i++)
        {
            for (int j = 0; j < zList.Count; j++)
            {
                float value = UnityEngine.Random.Range(0f, 1.0f);
                if (!GetRandomIndex(value, list, out int index))
                {
                    continue;
                }

                GameObject go = Instantiate<GameObject>(goList[index], parent);
                go.transform.position = new Vector3(i * lenStep, 0, zList[j]);
                go.transform.localScale = baseList[index].Scale;
                go.transform.Rotate(Vector3.up, baseList[index].Rotation.x);
                go.isStatic = baseList[index].ForceStatic;
            }
        }

        for (int i = 0; i < zCount; i++)
        {
            for (int j = 0; j < xList.Count; j++)
            {
                float value = UnityEngine.Random.Range(0f, 1.0f);
                if (!GetRandomIndex(value, list, out int index))
                {
                    continue;
                }

                GameObject go = Instantiate<GameObject>(goList[index], parent);
                go.transform.position = new Vector3(xList[j], 0, i * lenStep);
                go.transform.localScale = baseList[index].Scale;
                go.transform.Rotate(Vector3.up, baseList[index].Rotation.z);
                go.isStatic = baseList[index].ForceStatic;
            }
        }
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
        if (_selectTarget == null) return;
        int childCount = _selectTarget.transform.childCount;
        for (int i = childCount - 1; i >= 0; i--)
        {
            DestroyImmediate(_selectTarget.transform.GetChild(i).gameObject);
        }
    }
}