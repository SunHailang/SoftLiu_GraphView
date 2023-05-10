using System.Collections.Generic;
using LevelEditorTools.Nodes;
using UnityEngine;

namespace LevelEditorTools
{
    [System.Serializable]
    public class SceneContainer : ScriptableObject
    {
        public List<SceneNodeData> NodeDatas = new List<SceneNodeData>();
        public List<SceneNodeLinkData> NodeLinkDatas = new List<SceneNodeLinkData>();
        
        // Node 数据
        public List<SceneScriptable> NodeSceneDatas = new List<SceneScriptable>();
        public List<GroundScriptable> NodeGroundDatas = new List<GroundScriptable>();
        public List<DoorScriptable> NodeDoorDatas = new List<DoorScriptable>();
        public List<ObstacleScriptable> NodeObstacleDatas = new List<ObstacleScriptable>();
        public List<WallScriptable> NodeWallDatas = new List<WallScriptable>();
        public List<GameObjectScriptable> NodeGameObjectDatas = new List<GameObjectScriptable>();
    }
}