using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using LevelEditorTools.Nodes;

namespace LevelEditorTools.Editor.Nodes
{
    public class DoorNode : BaseNode
    {
        public DoorNode()
        {
            Port input = GraphViewUtils.GetInstantiatePort(this, Orientation.Vertical, Direction.Input, Port.Capacity.Single, typeof(int));
            this.inputContainer.Add(input);
            Port output = GraphViewUtils.GetInstantiatePort(this, Orientation.Vertical, Direction.Output, Port.Capacity.Single, typeof(GameObjectNode));
            this.outputContainer.Add(output);

            _state = new DoorScriptable();
            this.RefreshPorts();
            this.RefreshExpandedState();
        }

        public override bool DrawInspectorGUI()
        {
            bool hasChange = base.DrawInspectorGUI();
            if (_state is DoorScriptable scriptable)
            {
                Vector3 size = EditorGUILayout.Vector3Field("DoorSize", scriptable.DoorSize);
                if (scriptable.DoorSize != size)
                {
                    scriptable.DoorSize = size;
                    hasChange = true;
                }
            }

            return hasChange;
        }
    }
}