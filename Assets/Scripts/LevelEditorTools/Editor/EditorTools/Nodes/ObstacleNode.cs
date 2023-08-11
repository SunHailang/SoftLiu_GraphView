using EditorUtils;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using LevelEditorTools.Nodes;
using UnityEngine.UIElements;

namespace LevelEditorTools.Editor.Nodes
{
    public class ObstacleNode : BaseNode
    {
        public ObstacleNode()
        {
           Port input = GraphViewUtils.GetInstantiatePort(this, Orientation.Vertical, Direction.Input, Port.Capacity.Single, typeof(int));
           this.inputContainer.Add(input);
           Port output = GraphViewUtils.GetInstantiatePort(this, Orientation.Vertical, Direction.Output, Port.Capacity.Multi, typeof(GameObjectNode));
           this.outputContainer.Add(output);

           _state = new ObstacleScriptable();
        }

        protected override void EditorScriptContextMenu(DropdownMenuAction obj)
        {
            GraphViewUtils.OpenCodeEditor("ObstacleNode");
        }

        public override bool DrawInspectorGUI()
        {
            bool hasChange = base.DrawInspectorGUI();
            if (_state is ObstacleScriptable scriptable)
            {
                int seed = EditorGUILayout.IntField("Seed", scriptable.Seed, GUILayout.ExpandWidth(true));
                if (scriptable.Seed != seed)
                {
                    scriptable.Seed = seed;
                    hasChange = true;
                }
            }

            return hasChange;
        }
    }
}