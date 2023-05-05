using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using LevelEditorTools.Nodes;

namespace LevelEditorTools.Editor.Nodes
{
    public class DoorNode : BaseNode
    {
        private readonly DoorScriptable _scriptable;

        public DoorNode()
        {
            Port input = GraphViewUtils.GetInstantiatePort(this, Orientation.Vertical, Direction.Input, Port.Capacity.Single, typeof(int));
            this.inputContainer.Add(input);
            Port output = GraphViewUtils.GetInstantiatePort(this, Orientation.Vertical, Direction.Output, Port.Capacity.Single, typeof(GameObjectNode));
            this.outputContainer.Add(output);

            _scriptable = new DoorScriptable();
            State = _scriptable;
            this.RefreshPorts();
            this.RefreshExpandedState();
        }

        public override void DrawInspectorGUI()
        {
            base.DrawInspectorGUI();
            _scriptable.DoorSize = EditorGUILayout.Vector3Field("DoorSize", _scriptable.DoorSize);
        }
    }
}