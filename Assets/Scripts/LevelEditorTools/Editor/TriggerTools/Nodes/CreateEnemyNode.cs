using System;
using LevelEditorTools.Nodes;
using UnityEditor;
using UnityEngine;

namespace LevelEditorTools.Editor.Nodes
{
    public class CreateEnemyNode : BaseTriggerNode
    {
        public CreateEnemyNode() : base(true)
        {
            _state = new CreateEnemyScriptable();
        }

        public override bool DrawInspectorGUI()
        {
            bool hasChange = base.DrawInspectorGUI();
            if (_state is CreateEnemyScriptable scriptable)
            {
                Vector3 enemyPos = EditorGUILayout.Vector3Field("EnemyPosition", scriptable.EnemyPosition);
                if (scriptable.EnemyPosition != enemyPos)
                {
                    scriptable.EnemyPosition = enemyPos;
                    hasChange = true;
                }

                if (!scriptable.IsOnce)
                {
                    bool timing = EditorGUILayout.Toggle("TimingTrigger", scriptable.TimingTrigger);
                    if (scriptable.TimingTrigger != timing)
                    {
                        scriptable.TimingTrigger = timing;
                        hasChange = true;
                    }

                    if (timing)
                    {
                        float interval = EditorGUILayout.FloatField("TimeInterval", scriptable.TimeInterval);
                        if (Math.Abs(scriptable.TimeInterval - interval) > 0)
                        {
                            scriptable.TimeInterval = interval;
                            hasChange = true;
                        }
                    }
                }
                else
                {
                    scriptable.TimingTrigger = false;
                }
            }

            return hasChange;
        }
    }
}