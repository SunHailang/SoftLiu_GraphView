using System;
using GraphEditor.Nodes;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace GraphEditor.GraphViews
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

        private void OnMouseDownCallback(MouseDownEvent evt)
        {
            if (evt.button == 1)
            {
                // 右键按下 缓存当前鼠标相对窗体的坐标点
                Vector2 localPos = evt.localMousePosition;
                GenericMenu menu = new GenericMenu();
                menu.AddItem(new GUIContent("SceneNode"), false, () => { CreateNode<SceneNode>(localPos); });
                menu.AddItem(new GUIContent("GroundNode"), false, () => { CreateNode<GroundNode>(localPos); });
                menu.AddItem(new GUIContent("ObstacleNode"), false, () => { CreateNode<ObstacleNode>(localPos); });
                menu.AddItem(new GUIContent("WallNode"), false, () => { CreateNode<WallNode>(localPos); });
                menu.AddItem(new GUIContent("GameObjectNode"), false, () =>
                {
                    GameObjectNode node = CreateNode<GameObjectNode>(localPos);
                    node.RefreshTempleGo(window);
                });

                menu.ShowAsContext();
                Event.current.Use();
            }
        }

        private void OnMouseUpCallback(MouseUpEvent evt)
        {
            //Debug.Log($"OnMouseUpCallback:{evt.button}");
        }

        public T CreateNode<T>(Vector2 pos) where T : BaseNode, new()
        {
            Type type = typeof(T);
            string title = type.Name;
            string guid = System.Guid.NewGuid().ToString();
            return CreateNode<T>(pos, title, guid);
        }

        public T CreateNode<T>(Vector2 pos, string title, string guid) where T : BaseNode, new()
        {
            T node = new T();
            //添加Component，关联节点
            node.onSelected += OnNodeSelected;
            if (string.IsNullOrEmpty(title))
            {
                Type type = typeof(T);
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

        private void OnNodeSelected(BaseNode node, bool selected)
        {
            onNodeSelected?.Invoke(node, selected);
        }
    }
}