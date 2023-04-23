using System;
using System.Collections;
using System.Collections.Generic;
using Editor.Nodes;
using Unity.VisualScripting;
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

            // 按照父级的宽高全屏填充
            //this.StretchToParentSize();

            // 滚轮
            this.SetupZoom(ContentZoomer.DefaultMinScale, ContentZoomer.DefaultMaxScale);
            // 可以选中单个Node
            this.AddManipulator(new SelectionDragger());
            // 按住 Alt键 可以拖动
            this.AddManipulator(new ContentDragger());
            // 可以框选多个Node
            this.AddManipulator(new RectangleSelector());
            // Import USS  SceneGraphView.uss
            var styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>(AssetDatabase.GUIDToAssetPath("73411fe8094701f49b6a65893deb79fa"));
            styleSheets.Add(styleSheet);

            this.RegisterCallback<MouseDownEvent>(OnMouseDownCallback, TrickleDown.NoTrickleDown);
            this.RegisterCallback<MouseUpEvent>(OnMouseUpCallback, TrickleDown.TrickleDown);

        }


        private void OnMouseDownCallback(MouseDownEvent evt)
        {
            if(evt.button == 1)
            {
                // 右键按下
                Debug.Log($"OnMouseDownCallback:{evt.button}");
            }
        }

        private void OnMouseUpCallback(MouseUpEvent evt)
        {
            //Debug.Log($"OnMouseUpCallback:{evt.button}");
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