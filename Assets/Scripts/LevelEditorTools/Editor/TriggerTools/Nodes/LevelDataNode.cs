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


        public override bool DrawInspectorGUI()
        {
            bool hasChange = base.DrawInspectorGUI();
            string name = EditorGUILayout.TextField("LevelName", _scriptable.LevelName, GUILayout.ExpandWidth(true));
            if (_scriptable.LevelName != name)
            {
                _scriptable.LevelName = name;
                hasChange = true;
            }
            Vector3 pos= EditorGUILayout.Vector3Field("LevelPosition", _scriptable.LevelPosition);
            if (_scriptable.LevelPosition != pos)
            {
                _scriptable.LevelPosition = pos;
                hasChange = true;
            }
             Vector3 scale = EditorGUILayout.Vector3Field("LevelScale", _scriptable.LevelScale);
            if (_scriptable.LevelScale != scale)
            {
                _scriptable.LevelScale = scale;
                hasChange = true;
            }
            return hasChange;
        }
    }
}