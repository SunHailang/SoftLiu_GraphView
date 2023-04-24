using System.Collections.Generic;
using GraphEditor.Nodes;
using UnityEngine;

namespace GraphEditor
{
    [System.Serializable]
    public class SceneContainer : ScriptableObject
    {
        public List<SceneNodeData> NodeDatas = new List<SceneNodeData>();
        public List<SceneNodeLinkData> NodeLinkDatas = new List<SceneNodeLinkData>();
        
        // Node 数据
        [Header("Node 数据")] public List<SceneScriptable> NodeSceneDatas = new List<SceneScriptable>();
        public List<GroundScriptable> NodeGroundDatas = new List<GroundScriptable>();
        public List<GameObjectScriptable> NodeGameObjectDatas = new List<GameObjectScriptable>();
    }
}