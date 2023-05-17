using System;
using LevelEditorTools.Nodes;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(SceneNodeManager), true)]
public class SceneNodeManagerEditor : Editor
{
    private SceneNodeManager _manager;

    private void OnEnable()
    {
        if (Selection.activeObject is GameObject go)
        {
            _manager = go.GetComponent<SceneNodeManager>();
        }
    }


    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        if (_manager != null)
        {
            if (GUILayout.Button("Create", GUILayout.Height(35)))
            {
                _manager.Destroy();
                _manager.Create();
            }

            if (GUILayout.Button("Destroy", GUILayout.Height(35)))
            {
                _manager.Destroy();
            }
        }
    }
}