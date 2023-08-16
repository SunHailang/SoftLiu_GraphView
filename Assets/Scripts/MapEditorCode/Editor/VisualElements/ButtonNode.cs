using System;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using LevelEditorTools.Nodes;
using UnityEngine.UIElements;

namespace MapEditor
{
    public class ButtonNode : Node
    {
        public ButtonNode()
        {
            var image = Texture2D.redTexture;
            this.style.backgroundImage = image;
        }
    }
}