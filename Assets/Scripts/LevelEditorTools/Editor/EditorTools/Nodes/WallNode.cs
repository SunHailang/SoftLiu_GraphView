using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using LevelEditorTools.Nodes;

namespace LevelEditorTools.Editor.Nodes
{
    public class WallNode : BaseNode
    {
        private readonly WallScriptable _scriptable;

        public WallNode()
        {
            Port input = GraphViewUtils.GetInstantiatePort(this, Orientation.Vertical, Direction.Input, Port.Capacity.Single, typeof(int));
            this.inputContainer.Add(input);
            Port output = GraphViewUtils.GetInstantiatePort(this, Orientation.Vertical, Direction.Output, Port.Capacity.Multi, typeof(GameObjectNode));
            this.outputContainer.Add(output);

            _scriptable = new WallScriptable();

            State = _scriptable;
        }

        public override bool DrawInspectorGUI()
        {
            bool hasChange = base.DrawInspectorGUI();
            Vector3 size = EditorGUILayout.Vector3Field("WallSize", _scriptable.WallSize);
            if (_scriptable.WallSize != size)
            {
                _scriptable.WallSize = size;
                hasChange = true;
            }

            int seed = EditorGUILayout.IntField("Seed", _scriptable.Seed, GUILayout.ExpandWidth(true));
            if (_scriptable.Seed != seed)
            {
                _scriptable.Seed = seed;
                hasChange = true;
            }

            return hasChange;
        }
    }
}