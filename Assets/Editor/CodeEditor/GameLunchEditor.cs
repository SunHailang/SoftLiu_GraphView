using System;
using GraphEditor;
using GraphEditor.Nodes;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(GameLunch), true)]
public class GameLunchEditor : UnityEditor.Editor
{
    private SceneContainer _sceneContainer;

    private GameObject _selectTarget;

    private void OnEnable()
    {
        _selectTarget = Selection.activeObject as GameObject;
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        _sceneContainer = EditorGUILayout.ObjectField("SceneContainer", _sceneContainer, typeof(SceneContainer), false) as SceneContainer;
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
            go.transform.position = data.ScenePostion;
            // 2. 查找这个 Scene 的子节点
            
        }
    }

    private void DestroyScene()
    {
        if(_selectTarget == null) return;
        int childCount = _selectTarget.transform.childCount;
        for (int i = childCount - 1; i >= 0; i--)
        {
            DestroyImmediate(_selectTarget.transform.GetChild(i).gameObject);
        }
    }
}