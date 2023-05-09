using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace LevelEditorTools.Nodes
{
    [System.Serializable]
    public class CreateEnemyScriptable : BaseTriggerScriptable
    {
        public bool TimingTrigger = false;
        public float TimeInterval;
        public Vector3 EnemyPosition = Vector3.zero;
    }
}