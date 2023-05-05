using UnityEngine;

namespace LevelEditorTools.Nodes
{
    [System.Serializable]
    public class WallScriptable : BaseScriptable
    {
        public int Seed = 0;
        // 墙体的长宽
        public Vector3 WallSize = Vector3.one;
    }
}