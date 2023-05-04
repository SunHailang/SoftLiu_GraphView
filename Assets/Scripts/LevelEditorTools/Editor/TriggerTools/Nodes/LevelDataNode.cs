using LevelEditorTools.Nodes;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace LevelEditorTools.Editor.Nodes
{
    public class LevelDataNode : BaseNode
    {
        private LevelDataScriptable _scriptable;
        
        public LevelDataNode()
        {
            Port input = GraphViewUtils.GetInstantiatePort(this, Orientation.Vertical, Direction.Input, Port.Capacity.Multi, typeof(BoxTriggerNode));
            this.inputContainer.Add(input);
            
            this.RefreshExpandedState();
            this.RefreshPorts();

            _scriptable = new LevelDataScriptable();
            State = _scriptable;
        }


        public override void DrawInspectorGUI()
        {
            base.DrawInspectorGUI();
            _scriptable.LevelName = EditorGUILayout.TextField("LevelName", _scriptable.LevelName, GUILayout.ExpandWidth(true));
        }
    }
}