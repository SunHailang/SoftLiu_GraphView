using System;
using LevelEditorTools.Editor.Nodes;
using System.Collections.Generic;
using GraphEditor.GraphViews;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace LevelEditorTools.GraphViews
{
    public class SceneGraphView : GraphView
    {
        public new class UxmlFactory : UxmlFactory<SceneGraphView, UxmlTraits>
        {
        }

        public event Action<BaseNode, bool> onNodeSelected;
        public GraphViewWindw window;

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

            graphViewChanged = OnGraphViewChanged;

            LevelNodeProvider providerNode = ScriptableObject.CreateInstance<LevelNodeProvider>();
            providerNode.OnSelectEntryHandler += OnMenuSelectEntry;
            nodeCreationRequest += context => { SearchWindow.Open(new SearchWindowContext(context.screenMousePosition), providerNode); };
        }

        private GraphViewChange OnGraphViewChanged(GraphViewChange change)
        {
            Debug.Log($"SceneGraphView Changed!");
            
            if (change.edgesToCreate != null)
            {
                // 有 edge 创建
                window.SetUnsaveChange(true);
            }

            if (change.elementsToRemove != null)
            {
                // 有元素被移除
                window.SetUnsaveChange(true);
            }

            if (change.movedElements != null)
            {
                // 有元素被移动
                window.SetUnsaveChange(true);
            }

            return change;
        }

        private bool OnMenuSelectEntry(SearchTreeEntry searchTreeEntry, SearchWindowContext context)
        {
            if (searchTreeEntry.userData is Type type)
            {
                //获取鼠标位置
                var windowRoot = window.rootVisualElement;
                var windowMousePosition = windowRoot.ChangeCoordinatesTo(windowRoot.parent, context.screenMousePosition - window.position.position);
                var graphMousePosition = contentViewContainer.WorldToLocal(windowMousePosition);
                BaseNode node = CreateNode(type, graphMousePosition);
                if (node is GameObjectNode goNode)
                {
                    goNode.RefreshTempleGo(window);
                }

                bool active = node != null;
                if (active)
                {
                    window.SetUnsaveChange(true);
                }

                return active;
            }

            return false;
        }

        public BaseNode CreateNode(Type type, Vector2 pos)
        {
            if (type != null)
            {
                string title = type.Name;
                string guid = System.Guid.NewGuid().ToString();
                return CreateNode(type, pos, title, guid);
            }

            return null;
        }

        public BaseNode CreateNode(Type type, Vector2 pos, string title, string guid)
        {
            if (type != null && Activator.CreateInstance(type) is BaseNode node)
            {
                node.onSelected += OnNodeSelected;
                if (string.IsNullOrEmpty(title))
                {
                    title = type.Name;
                    Debug.LogError($"CreateNode Error: title is Empty.");
                }

                node.name = title;
                node.SetTitle(title);
                node.SetGuid(guid);
                // 屏幕坐标转本地坐标
                node.SetPosition(new Rect(pos, node.GetPosition().size));

                node.RefreshExpandedState();
                node.RefreshPorts();
                this.AddElement(node);
                return node;
            }

            return null;
        }

        public override List<Port> GetCompatiblePorts(Port startPort, NodeAdapter nodeAdapter)
        {
            List<Port> compatiblePorts = new List<Port>();

            ports.ForEach((port) =>
            {
                // 对每一个 GraphView 里面的Port判断规则：
                // 1. port不可以与自身相连
                // 2. 同一个节点的port之间不可以相连
                if (port != startPort && port.node != startPort.node && startPort.direction != port.direction && startPort.portType == port.portType)
                {
                    compatiblePorts.Add(port);
                }
            });
            return compatiblePorts;
        }


        private void OnNodeSelected(BaseNode node, bool selected)
        {
            onNodeSelected?.Invoke(node, selected);
        }
    }
}