using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using LevelEditorTools.Nodes;

namespace LevelEditorTools.Editor.Nodes
{
    public class ObstacleNode : BaseNode
    {
        private readonly ObstacleScriptable _obstacle;
        public ObstacleNode()
        {
           Port input = GraphViewUtils.GetInstantiatePort(this, Orientation.Vertical, Direction.Input, Port.Capacity.Single, typeof(int));
           this.inputContainer.Add(input);
           Port output = GraphViewUtils.GetInstantiatePort(this, Orientation.Vertical, Direction.Output, Port.Capacity.Multi, typeof(GameObjectNode));
           this.outputContainer.Add(output);

           _obstacle = new ObstacleScriptable();
           State = _obstacle;
        }


        public override bool DrawInspectorGUI()
        {
            bool hasChange = base.DrawInspectorGUI();
            int seed = EditorGUILayout.IntField("Seed", _obstacle.Seed, GUILayout.ExpandWidth(true));
            if (_obstacle.Seed != seed)
            {
                _obstacle.Seed = seed;
                hasChange = true;
            }
            return hasChange;
        }
    }
}