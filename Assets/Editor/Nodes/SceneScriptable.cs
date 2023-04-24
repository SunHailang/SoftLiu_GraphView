using UnityEngine;

namespace GraphEditor.Nodes
{
    [System.Serializable]
    public class SceneScriptable : BaseScriptable
    {
        [Tooltip("世界坐标系")]
        public Vector3 ScenePostion = Vector3.zero;
    }
}