using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using LevelEditorTools.Nodes;

namespace LevelEditorTools.Editor.Nodes
{
    public class ObstacleNode : BaseNode
    {
        private ObstacleScriptable _obstacle;
        public ObstacleNode()
        {
           Port input = GraphViewUtils.GetInstantiatePort(this, Orientation.Vertical, Direction.Input, Port.Capacity.Single, typeof(int));
           this.inputContainer.Add(input);
           Port output = GraphViewUtils.GetInstantiatePort(this, Orientation.Vertical, Direction.Output, Port.Capacity.Multi, typeof(int));
           this.outputContainer.Add(output);

           _obstacle = new ObstacleScriptable();
           State = _obstacle;
        }


        public override void DrawInspectorGUI()
        {
            base.DrawInspectorGUI();
            _obstacle.Seed = EditorGUILayout.IntField("Seed", _obstacle.Seed, GUILayout.ExpandWidth(true));
        }
    }
}