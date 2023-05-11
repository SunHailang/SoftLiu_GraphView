
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

        if (GUILayout.Button("Load") && _mono != null)
        {
            _mono.LoadPosition();
        }
        if (GUILayout.Button("Save") && _mono != null)
        {
            _mono.SetPosition();
        }
        
    }
}