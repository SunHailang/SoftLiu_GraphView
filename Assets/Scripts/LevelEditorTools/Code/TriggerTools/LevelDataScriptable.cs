using System.Collections.Generic;
using UnityEngine;

namespace LevelEditorTools.Nodes
{
    [System.Serializable]
    public class LevelDataScriptable : BaseScriptable
    {
        public string LevelName = "";
        public Vector3 LevelPosition = Vector3.zero;
        public Vector3 LevelScale = Vector3.one;
    }
}