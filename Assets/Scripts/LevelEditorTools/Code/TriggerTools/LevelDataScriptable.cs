using System.Collections.Generic;
using UnityEngine;

namespace LevelEditorTools.Nodes
{
    public class LevelDataScriptable : BaseScriptable
    {
        public string LevelName = "";
        public List<BoxTriggerScriptable> BoxTriggerList = new List<BoxTriggerScriptable>();
    }
}