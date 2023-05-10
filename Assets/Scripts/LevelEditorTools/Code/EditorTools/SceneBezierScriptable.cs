using System.Collections.Generic;
using UnityEngine;

namespace LevelEditorTools.Nodes
{
    [System.Serializable]
    public class SceneBezierScriptable : BaseScriptable
    {
        public Vector3 StartPosition;
        public Vector3 EndPosition;

        public float Width;
        
        public List<Vector3> ControlPosition = new List<Vector3>();

    }
}