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

        public override bool DrawInspectorGUI()
        {
            bool hasChange = base.DrawInspectorGUI();
            Vector3 scale = EditorGUILayout.Vector3Field("SceneScale", _scriptable.SceneScale, GUILayout.ExpandWidth(true));
            if (_scriptable.SceneScale != scale)
            {
                _scriptable.SceneScale = scale;
                hasChange = true;
            }

            Vector3 pos = EditorGUILayout.Vector3Field("ScenePosition", _scriptable.ScenePosition, GUILayout.ExpandWidth(true));
            if (_scriptable.ScenePosition != pos)
            {
                _scriptable.ScenePosition = pos;
                hasChange = true;
            }

            return hasChange;
        }
    }
}