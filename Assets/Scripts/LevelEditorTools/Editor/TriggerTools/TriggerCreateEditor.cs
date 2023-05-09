using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(TriggerCreateMono))]
public class TriggerCreateEditor : UnityEditor.Editor
{
    private TriggerCreateMono _trigger;

    private void OnEnable()
    {
        if (Selection.activeObject is GameObject go)
        {
            _trigger = go.GetComponent<TriggerCreateMono>();
        }
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        
        serializedObject.Update();
        
        if(GUILayout.Button("刷新"))
        {
            if (_trigger != null)
            {
                _trigger.UpdateTrigger();
            }
        }
        
        serializedObject.ApplyModifiedProperties();
    }
}
