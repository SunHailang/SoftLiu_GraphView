using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using LevelEditorTools.Nodes;

namespace LevelEditorTools.Editor.Nodes
{
    public class SceneNode : BaseNode
    {
        private readonly SceneScriptable _scriptable;
        public SceneNode()
        {
            Port output = GraphViewUtils.GetInstantiatePort(this, Orientation.Vertical, Direction.Output, Port.Capacity.Multi, typeof(int));
            this.outputContainer.Add(output);

            _scriptable = new SceneScriptable();
            State = _scriptable;
        }

        public override void DrawInspectorGUI()
        {
            base.DrawInspectorGUI();
            _scriptable.SceneScale = EditorGUILayout.Vector3Field("SceneScale", _scriptable.SceneScale, GUILayout.ExpandWidth(true));
            _scriptable.ScenePosition = EditorGUILayout.Vector3Field("ScenePosition", _scriptable.ScenePosition, GUILayout.ExpandWidth(true));
        }
    }
}