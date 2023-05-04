using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using LevelEditorTools.Nodes;

namespace LevelEditorTools.Editor.Nodes
{
    public class SceneNode : BaseNode
    {
        private SceneScriptable scene = null;
        public SceneNode()
        {
            Port output = GraphViewUtils.GetInstantiatePort(this, Orientation.Vertical, Direction.Output, Port.Capacity.Multi, typeof(int));
            this.outputContainer.Add(output);

            scene = new SceneScriptable();
            State = scene;
        }

        public override void DrawInspectorGUI()
        {
            base.DrawInspectorGUI();
            scene.SceneScale = EditorGUILayout.Vector3Field("SceneScale", scene.SceneScale, GUILayout.ExpandWidth(true));
            scene.ScenePosition = EditorGUILayout.Vector3Field("ScenePosition", scene.ScenePosition, GUILayout.ExpandWidth(true));
        }
    }
}