using EditorUtils;
using LevelEditorTools.Nodes;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace LevelEditorTools.Editor.Nodes
{
    public class BaseTriggerNode : BaseNode
    {
        public BaseTriggerNode() : base()
        {
            CreatePort();

            _state = new BaseTriggerScriptable();
        }

        public BaseTriggerNode(bool isBase) : base()
        {
            CreatePort();
        }

        private void CreatePort()
        {
            Port input = GraphViewUtils.GetInstantiatePort(this, Orientation.Horizontal, Direction.Input, Port.Capacity.Single, typeof(ConditionTriggerNode));
            this.inputContainer.Add(input);

            Port output = GraphViewUtils.GetInstantiatePort(this, Orientation.Horizontal, Direction.Output, Port.Capacity.Single, typeof(BaseTriggerNode));
            this.outputContainer.Add(output);
            this.RefreshExpandedState();
            this.RefreshPorts();
        }

        public override bool DrawInspectorGUI()
        {
            bool hasChange = base.DrawInspectorGUI();
            if (_state is BaseTriggerScriptable scriptable)
            {
                int id = EditorGUILayout.IntField("EventID", scriptable.EventID);
                if (scriptable.EventID != id)
                {
                    scriptable.EventID = id;
                    hasChange = true;
                }

                Vector3 pos = EditorGUILayout.Vector3Field("Position", scriptable.Position, GUILayout.ExpandWidth(true));
                if (scriptable.Position != pos)
                {
                    scriptable.Position = pos;
                    hasChange = true;
                }

                Vector3 scale = EditorGUILayout.Vector3Field("Scale", scriptable.Scale, GUILayout.ExpandWidth(true));
                if (scriptable.Scale != scale)
                {
                    scriptable.Scale = scale;
                    hasChange = true;
                }

                TriggerStateEnum state = (TriggerStateEnum) EditorGUILayout.EnumPopup("TriggerState", scriptable.TriggerState);
                if (scriptable.TriggerState != state)
                {
                    scriptable.TriggerState = state;
                    hasChange = true;
                }

                if ((scriptable.TriggerState & TriggerStateEnum.EntryAndExist) > 0)
                {
                    bool isOnce = EditorGUILayout.Toggle("IsOnce", scriptable.IsOnce);
                    if (scriptable.IsOnce != isOnce)
                    {
                        scriptable.IsOnce = isOnce;
                        hasChange = true;
                    }
                }
                else
                {
                    scriptable.IsOnce = true;
                }
            }

            return hasChange;
        }
    }
}