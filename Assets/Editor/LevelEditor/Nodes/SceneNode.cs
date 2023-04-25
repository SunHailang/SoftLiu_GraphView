using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace GraphEditor.Nodes
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
            scene.ScenePostion = EditorGUILayout.Vector3Field("ScenePostion", scene.ScenePostion, GUILayout.ExpandWidth(true));
        }
    }
}