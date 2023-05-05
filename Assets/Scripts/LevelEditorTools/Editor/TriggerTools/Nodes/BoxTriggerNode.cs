using LevelEditorTools.Nodes;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace LevelEditorTools.Editor.Nodes
{
    public class BoxTriggerNode : BaseNode
    {
        private BoxTriggerScriptable _scriptable;
        
        public BoxTriggerNode()
        {
            Port output = GraphViewUtils.GetInstantiatePort(this, Orientation.Vertical, Direction.Output, Port.Capacity.Single, typeof(BoxTriggerNode));
            this.outputContainer.Add(output);
            this.RefreshExpandedState();
            this.RefreshPorts();

            _scriptable = new BoxTriggerScriptable();

            State = _scriptable;
        }

        public override void DrawInspectorGUI()
        {
            base.DrawInspectorGUI();

            _scriptable.EventID = EditorGUILayout.IntField("EventID", _scriptable.EventID);
            _scriptable.Position = EditorGUILayout.Vector3Field("Position", _scriptable.Position, GUILayout.ExpandWidth(true));
            _scriptable.Scale = EditorGUILayout.Vector3Field("Scale", _scriptable.Scale, GUILayout.ExpandWidth(true));
            _scriptable.TriggerState = (TriggerStateEnum) EditorGUILayout.EnumPopup("TriggerState", _scriptable.TriggerState);
            if ((_scriptable.TriggerState & TriggerStateEnum.EntryAndExist) > 0)
            {
                _scriptable.IsOnce = EditorGUILayout.Toggle("IsOnce", _scriptable.IsOnce);
            }
        }
    }
}