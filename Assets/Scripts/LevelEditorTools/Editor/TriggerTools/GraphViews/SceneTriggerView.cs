using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;
using LevelEditorTools.Editor.Nodes;

namespace LevelEditorTools.GraphViews
{
    public class SceneTriggerView : GraphView
    {
        public new class UxmlFactory : UxmlFactory<SceneTriggerView, UxmlTraits>
        {
        }
        
        public event Action<BaseNode, bool> onNodeSelected;

        public EditorWindow window;

        public SceneTriggerView()
        {
            Insert(0, new GridBackground());

            // 滚轮
            // this.SetupZoom(ContentZoomer.DefaultMinScale, ContentZoomer.DefaultMaxScale);
            this.SetupZoom(0.2f, 2.0f);
            this.AddManipulator(new ContentZoomer());
            // 可以选中单个Node
            this.AddManipulator(new SelectionDragger());
            // 按住 Alt键 可以拖动
            this.AddManipulator(new ContentDragger());
            // 可以框选多个Node
            this.AddManipulator(new RectangleSelector());

            string path = AssetDatabase.GUIDToAssetPath("73411fe8094701f49b6a65893deb79fa");
            StyleSheet styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>(path);
            this.styleSheets.Add(styleSheet);

            TriggerNodeProvider providerNode = ScriptableObject.CreateInstance<TriggerNodeProvider>();
            providerNode.OnSelectEntryHandler += OnMenuSelectEntry;

            nodeCreationRequest += context => { SearchWindow.Open(new SearchWindowContext(context.screenMousePosition), providerNode); };
        }
        
        public override List<Port> GetCompatiblePorts(Port startPort, NodeAdapter nodeAdapter)
        {
            List<Port> compatiblePorts = new List<Port>();

            ports.ForEach((port) =>
            {
                // 对每一个 GraphView 里面的Port判断规则：
                // 1. port不可以与自身相连
                // 2. 同一个节点的port之间不可以相连
                // 3. 相同Type的port不可相连
                if (port != startPort && port.node != startPort.node && startPort.direction != port.direction && startPort.portType == port.portType)
                {
                    compatiblePorts.Add(port);
                }
            });
            return compatiblePorts;
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
                return node != null;
            }

            return false;
        }

        public BaseNode CreateNode(Type type, Vector2 pos)
        {
            if (type != null)
            {
                return CreateNode(type, pos, type.Name, "");
            }

            return null;
        }
        
        public BaseNode CreateNode(Type type, Vector2 pos, string title, string guid)
        {
            if (type != null)
            {
                if (Activator.CreateInstance(type) is BaseNode node)
                {
                    node.SetTitle(title);
                    node.SetGuid(guid);
                    node.SetPosition(new Rect(pos, node.GetPosition().size));
                    this.AddElement(node);
                    node.onSelected += OnNodeSelected;
                    return node;
                }
            }

            return null;
        }
        
        private void OnNodeSelected(BaseNode node, bool selected)
        {
            onNodeSelected?.Invoke(node, selected);
        }
    }
}