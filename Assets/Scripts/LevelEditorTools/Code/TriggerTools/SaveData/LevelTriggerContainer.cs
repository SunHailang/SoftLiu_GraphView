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
        public List<BoxTriggerScriptable> BoxTriggerDatas = new List<BoxTriggerScriptable>();
    }
}