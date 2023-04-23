using Editor.Nodes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor.Experimental.GraphView;
using UnityEditor;

namespace Editor.Nodes
{

    public class GameObjectNode : BaseNode
    {

        public GameObjectNode()
        {
            // Ìí¼ÓÒ»¸ö input
            Port input = GraphViewUtils.GetInstantiatePort(this, Orientation.Vertical, Direction.Input, Port.Capacity.Single, typeof(bool));
            input.portName = "input";
            SetTitle(nameof(GameObjectNode));
            this.Add(input);

            GameObjectScriptable state = ScriptableObject.CreateInstance<GameObjectScriptable>();

            _state = state;
        }

    }
}