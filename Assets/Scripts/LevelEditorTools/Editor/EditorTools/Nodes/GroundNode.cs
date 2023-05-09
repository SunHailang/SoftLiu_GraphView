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
        private readonly GroundScriptable _scriptable;
        public GroundNode()
        {
            // 创建输入
            Port input = GraphViewUtils.GetInstantiatePort(this, Orientation.Vertical, Direction.Input, Port.Capacity.Single, typeof(int));
            this.inputContainer.Add(input);

            Port output = GraphViewUtils.GetInstantiatePort(this, Orientation.Vertical, Direction.Output, Port.Capacity.Multi, typeof(GameObjectNode));
            this.outputContainer.Add(output);

            _scriptable = new GroundScriptable();
            State = _scriptable;
        }

        public override bool DrawInspectorGUI()
        {
            bool hasChange = base.DrawInspectorGUI();
            Vector3 size = EditorGUILayout.Vector3Field("GroundSize", _scriptable.GroundSize);
            if (_scriptable.GroundSize != size)
            {
                _scriptable.GroundSize = size;
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