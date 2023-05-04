using UnityEngine;

namespace LevelEditorTools.Nodes
{
    [System.Serializable]
    public class GameObjectScriptable : BaseScriptable
    {
        public Vector3 Position = Vector3.zero;
        public Vector3 Rotation = Vector3.zero;
        public Vector3 Scale = Vector3.one;
        public string TemplateGo;

        [Range(0, 1)] public float Probability = 0.2f;

        public bool ForceStatic = true;
    }
}
