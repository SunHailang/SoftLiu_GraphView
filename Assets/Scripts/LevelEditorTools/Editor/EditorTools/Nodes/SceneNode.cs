using System;
using EditorUtils;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using LevelEditorTools.Nodes;
using UnityEngine.UIElements;

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
        protected override void EditorScriptContextMenu(DropdownMenuAction obj)
        {
            GraphViewUtils.OpenCodeEditor("SceneNode");
        }

        public override bool DrawInspectorGUI()
        {
            bool hasChange = base.DrawInspectorGUI();
            if (_state is SceneScriptable scriptable)
            {
                var active = EditorGUILayout.Toggle("IsActive", scriptable.IsActive);
                if (scriptable.IsActive != active)
                {
                    scriptable.IsActive = active;
                    hasChange = true;
                }
                
                var type = (SceneTypeEnum) EditorGUILayout.EnumPopup("SceneType", scriptable.SceneType);
                if (type != scriptable.SceneType)
                {
                    scriptable.SceneType = type;
                    hasChange = true;
                }

                if (type == SceneTypeEnum.Rectangle)
                {
                    var scale = EditorGUILayout.Vector3Field("SceneScale", scriptable.SceneScale, GUILayout.ExpandWidth(true));
                    if (scriptable.SceneScale != scale)
                    {
                        scriptable.SceneScale = scale;
                        hasChange = true;
                    }
                }
                else
                {
                    var radius = EditorGUILayout.FloatField("Radius", scriptable.Radius);
                    if (Math.Abs(scriptable.Radius - radius) > 0)
                    {
                        scriptable.Radius = radius;
                        hasChange = true;
                    }
                }

                var pos = EditorGUILayout.Vector3Field("ScenePosition", scriptable.ScenePosition, GUILayout.ExpandWidth(true));
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