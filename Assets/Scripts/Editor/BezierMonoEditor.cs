
using System;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(BezierMono), true)]
public class BezierMonoEditor : Editor
{
    private BezierMono _mono;
    private void OnEnable()
    {
        if(Selection.activeObject is GameObject go)
        {
            _mono = go.GetComponent<BezierMono>();
        }
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        if (GUILayout.Button("刷新") && _mono != null)
        {
            _mono.SetPosition();
        }
        
    }
}