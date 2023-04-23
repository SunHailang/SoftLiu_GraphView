using System;
using System.Collections;
using System.Collections.Generic;
using Editor.Nodes;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace Editor.GraphViews
{
    public class SceneGraphView : GraphView
    {
        public class UxmlFactor : UxmlFactory<SceneGraphView, UxmlTraits>
        {
        }

        public event Action<BaseNode, bool> onNodeSelected;
        public EditorWindow window;
        public SceneGraphView()
        {
            Insert(0, new GridBackground());

            this.AddManipulator(new SelectionDragger());
            this.AddManipulator(new ContentZoomer());
            this.AddManipulator(new ContentDragger());
            this.AddManipulator(new RectangleSelector());
            // Import USS  SceneGraphView.uss
            var styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>(AssetDatabase.GUIDToAssetPath("73411fe8094701f49b6a65893deb79fa"));
            styleSheets.Add(styleSheet);
        }


        public void CreateNode<T>(Vector2 pos) where T : BaseNode, new()
        {
            System.Type type = typeof(T);
            T node = new T();
            //添加Component，关联节点
            node.onSelected += OnNodeSelected;
            node.name = $"{type.Name}";
            node.SetTitle(type.Name);
            node.SetPosition(new Rect(pos, node.GetPosition().size));
            
            
            node.RefreshExpandedState();
            node.RefreshPorts();
            
            this.AddElement(node);
        }

        public void ResetNodeView()
        {

        }

        private void OnNodeSelected(BaseNode node, bool selected)
        {
            onNodeSelected?.Invoke(node, selected);
        }
    }
}