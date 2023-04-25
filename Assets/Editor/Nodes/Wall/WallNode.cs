using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace GraphEditor.Nodes
{
    public class WallNode : BaseNode
    {
        private WallScriptable _scriptable;
        
        public WallNode()
        {
            Port input = GraphViewUtils.GetInstantiatePort(this, Orientation.Vertical, Direction.Input, Port.Capacity.Single, typeof(int));
            this.inputContainer.Add(input);
            Port output = GraphViewUtils.GetInstantiatePort(this, Orientation.Vertical, Direction.Output, Port.Capacity.Multi, typeof(int));
            this.outputContainer.Add(output);

            _scriptable = new WallScriptable();

            State = _scriptable;
        }

        public override void DrawInspectorGUI()
        {
            base.DrawInspectorGUI();
            _scriptable.Seed = EditorGUILayout.IntField("Seed", _scriptable.Seed, GUILayout.ExpandWidth(true));
        }
    }
}