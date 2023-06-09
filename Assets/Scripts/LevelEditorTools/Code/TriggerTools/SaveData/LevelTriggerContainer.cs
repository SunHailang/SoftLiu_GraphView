using System.Collections.Generic;
using LevelEditorTools.Nodes;
using UnityEngine;

namespace LevelEditorTools.Save
{
    public class LevelTriggerContainer : ScriptableObject
    {
        public List<SceneNodeData> NodeDatas = new List<SceneNodeData>();
        public List<SceneNodeLinkData> NodeLinkDatas = new List<SceneNodeLinkData>();

        public List<LevelDataScriptable> LevelDatas = new List<LevelDataScriptable>();
        public List<ConditionTriggerScriptable> ConditionTriggerDatas = new List<ConditionTriggerScriptable>();
        public List<BaseTriggerScriptable> BoxTriggerDatas = new List<BaseTriggerScriptable>();
        public List<CreateEnemyScriptable> CreateEnemyDatas = new List<CreateEnemyScriptable>();
    }
}