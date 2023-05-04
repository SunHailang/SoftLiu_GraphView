using UnityEngine;
using UnityEngine.Serialization;

namespace LevelEditorTools.Nodes
{
    [System.Serializable]
    public class SceneScriptable : BaseScriptable
    {
        /// <summary>
        /// 设置场景的大小
        /// </summary>
        public Vector3 SceneScale = Vector3.one;

        /// <summary>
        /// 世界坐标系
        /// </summary>
        public Vector3 ScenePosition = Vector3.zero;
    }
}