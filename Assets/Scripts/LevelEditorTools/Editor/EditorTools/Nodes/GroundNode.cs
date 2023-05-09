using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using LevelEditorTools.Nodes;

namespace LevelEditorTools.Editor.Nodes
{
    /// <summary>
    /// 地形 Node 用于创建地形 所以 一个输入端口可以允许多个输入
    /// </summary>
    public class GroundNode : BaseNode
    {
        public GroundNode()
        {
            // 创建输入
            Port input = GraphViewUtils.GetInstantiatePort(this, Orientation.Vertical, Direction.Input, Port.Capacity.Single, typeof(int));
            this.inputContainer.Add(input);

            Port output = GraphViewUtils.GetInstantiatePort(this, Orientation.Vertical, Direction.Output, Port.Capacity.Multi, typeof(GameObjectNode));
            this.outputContainer.Add(output);

            _state = new GroundScriptable();
        }

        public override bool DrawInspectorGUI()
        {
            bool hasChange = base.DrawInspectorGUI();
            if (_state is GroundScriptable scriptable)
            {
                Vector3 size = EditorGUILayout.Vector3Field("GroundSize", scriptable.GroundSize);
                if (scriptable.GroundSize != size)
                {
                    scriptable.GroundSize = size;
                    hasChange = true;
                }

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