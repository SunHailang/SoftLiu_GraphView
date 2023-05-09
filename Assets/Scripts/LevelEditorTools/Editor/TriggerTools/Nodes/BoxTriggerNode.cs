using LevelEditorTools.Nodes;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace LevelEditorTools.Editor.Nodes
{
    public class BoxTriggerNode : BaseNode
    {
        private BoxTriggerScriptable _scriptable;

        public BoxTriggerNode()
        {
            Port input = GraphViewUtils.GetInstantiatePort(this, Orientation.Horizontal, Direction.Input, Port.Capacity.Single, typeof(ConditionTriggerNode));
            this.inputContainer.Add(input);

            Port output = GraphViewUtils.GetInstantiatePort(this, Orientation.Vertical, Direction.Output, Port.Capacity.Single, typeof(BoxTriggerNode));
            this.outputContainer.Add(output);
            this.RefreshExpandedState();
            this.RefreshPorts();

            _scriptable = new BoxTriggerScriptable();

            State = _scriptable;
        }

        public override bool DrawInspectorGUI()
        {
            bool hasChange = base.DrawInspectorGUI();

            int id = EditorGUILayout.IntField("EventID", _scriptable.EventID);
            if (_scriptable.EventID != id)
            {
                _scriptable.EventID = id;
                hasChange = true;
            }

            Vector3 pos = EditorGUILayout.Vector3Field("Position", _scriptable.Position, GUILayout.ExpandWidth(true));
            if (_scriptable.Position != pos)
            {
                _scriptable.Position = pos;
                hasChange = true;
            }

            Vector3 scale = EditorGUILayout.Vector3Field("Scale", _scriptable.Scale, GUILayout.ExpandWidth(true));
            if (_scriptable.Scale != scale)
            {
                _scriptable.Scale = scale;
                hasChange = true;
            }

            TriggerStateEnum state = (TriggerStateEnum) EditorGUILayout.EnumPopup("TriggerState", _scriptable.TriggerState);
            if (_scriptable.TriggerState != state)
            {
                _scriptable.TriggerState = state;
                hasChange = true;
            }

            if ((_scriptable.TriggerState & TriggerStateEnum.EntryAndExist) > 0)
            {
                bool isOnce = EditorGUILayout.Toggle("IsOnce", _scriptable.IsOnce);
                if (_scriptable.IsOnce != isOnce)
                {
                    _scriptable.IsOnce = isOnce;
                    hasChange = true;
                }
            }

            Vector3 enemyPos = EditorGUILayout.Vector3Field("EnemyPosition", _scriptable.EnemyPosition);
            if (_scriptable.EnemyPosition != enemyPos)
            {
                _scriptable.EnemyPosition = enemyPos;
                hasChange = true;
            }

            return hasChange;
        }
    }
}