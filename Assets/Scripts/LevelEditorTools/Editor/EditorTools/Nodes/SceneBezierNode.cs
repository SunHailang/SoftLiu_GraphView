using System;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using LevelEditorTools.Nodes;
using UnityEngine.UIElements;

namespace LevelEditorTools.Editor.Nodes
{
    public class SceneBezierNode : BaseNode
    {
        public SceneBezierNode()
        {
            Port output = GraphViewUtils.GetInstantiatePort(this, Orientation.Vertical, Direction.Output, Port.Capacity.Multi, typeof(int));
            this.outputContainer.Add(output);

            _state = new SceneBezierScriptable();
        }

        public override void BuildContextualMenu(ContextualMenuPopulateEvent evt)
        {
            base.BuildContextualMenu(evt);
            // 打开一个 绘制 贝塞尔曲线的 Window
            evt.menu.AppendAction("Open Bezier Window", OpenBezierWindow);
        }

        private void OpenBezierWindow(DropdownMenuAction obj)
        {
            
        }

        protected override void EditorScriptContextMenu(DropdownMenuAction obj)
        {
            EditorUtility.OpenPropertyEditor(null);
        }

        public override bool DrawInspectorGUI()
        {
            bool hasChange = base.DrawInspectorGUI();
            if (_state is SceneBezierScriptable scriptable)
            {
                Vector3 start = EditorGUILayout.Vector3Field("StartPosition", scriptable.StartPosition);
                if (scriptable.StartPosition != start)
                {
                    scriptable.StartPosition = start;
                    hasChange = true;
                }

                Vector3 end = EditorGUILayout.Vector3Field("EndPosition", scriptable.EndPosition);
                if (scriptable.EndPosition != end)
                {
                    scriptable.EndPosition = end;
                    hasChange = true;
                }

                float width = EditorGUILayout.FloatField("Width", scriptable.Width);
                if (Math.Abs(scriptable.Width - width) > 0)
                {
                    scriptable.Width = width;
                    hasChange = true;
                }
            }


            return hasChange;
        }
    }
}