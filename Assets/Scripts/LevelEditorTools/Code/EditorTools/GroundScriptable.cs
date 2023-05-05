using UnityEngine;

namespace LevelEditorTools.Nodes
{
    [System.Serializable]
    public class GroundScriptable : BaseScriptable
    {
        public int Seed = int.MaxValue;
        // 地板的长宽
        public Vector3 GroundSize = Vector3.one;
    }
}