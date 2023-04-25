using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace GraphEditor.LevelTrigger
{
    public class SceneTriggerView : GraphView
    {
        public class UxmlFactory : UxmlFactory<SceneTriggerView, UxmlTraits>
        {
        }

        public SceneTriggerView()
        {
            Insert(0, new GridBackground());
            
            
        }
        
    }
}