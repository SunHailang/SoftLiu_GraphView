using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using LevelEditorTools.Nodes;

namespace LevelEditorTools.Editor.Nodes
{
    public class SceneNode : BaseNode
    {
        public SceneNode()
        {
            Port output = GraphViewUtils.GetInstantiatePort(this, Orientation.Vertical, Direction.Output, Port.Capacity.Multi, typeof(int));
            this.outputContainer.Add(output);

            _state = new SceneScriptable();
        }

        public override bool DrawInspectorGUI()
        {
            bool hasChange = base.DrawInspectorGUI();
            if (_state is SceneScriptable scriptable)
            {
                Vector3 scale = EditorGUILayout.Vector3Field("SceneScale", scriptable.SceneScale, GUILayout.ExpandWidth(true));
                if (scriptable.SceneScale != scale)
                {
                    scriptable.SceneScale = scale;
                    hasChange = true;
                }

                Vector3 pos = EditorGUILayout.Vector3Field("ScenePosition", scriptable.ScenePosition, GUILayout.ExpandWidth(true));
                if (scriptable.ScenePosition != pos)
                {
                    scriptable.ScenePosition = pos;
                    hasChange = true;
                }
            }

            return hasChange;
        }
    }
}