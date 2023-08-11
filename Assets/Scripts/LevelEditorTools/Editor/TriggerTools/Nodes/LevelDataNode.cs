using EditorUtils;
using LevelEditorTools.Nodes;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace LevelEditorTools.Editor.Nodes
{
    public class LevelDataNode : BaseNode
    {
        public LevelDataNode()
        {
            Port input = GraphViewUtils.GetInstantiatePort(this, Orientation.Horizontal, Direction.Input, Port.Capacity.Multi, typeof(BaseTriggerNode));
            this.inputContainer.Add(input);
            
            this.RefreshExpandedState();
            this.RefreshPorts();

            _state = new LevelDataScriptable();
        }


        public override bool DrawInspectorGUI()
        {
            bool hasChange = base.DrawInspectorGUI();
            if (_state is LevelDataScriptable scriptable)
            {
                string name = EditorGUILayout.TextField("LevelName", scriptable.LevelName, GUILayout.ExpandWidth(true));
                if (scriptable.LevelName != name)
                {
                    scriptable.LevelName = name;
                    hasChange = true;
                }

                Vector3 pos = EditorGUILayout.Vector3Field("LevelPosition", scriptable.LevelPosition);
                if (scriptable.LevelPosition != pos)
                {
                    scriptable.LevelPosition = pos;
                    hasChange = true;
                }

                Vector3 scale = EditorGUILayout.Vector3Field("LevelScale", scriptable.LevelScale);
                if (scriptable.LevelScale != scale)
                {
                    scriptable.LevelScale = scale;
                    hasChange = true;
                }
            }

            return hasChange;
        }
    }
}