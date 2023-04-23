using Editor.Nodes;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.UIElements;

namespace Editor.GraphViews
{
    public class GraphViewWindw : EditorWindow
    {
        [MenuItem("Tools/GraphViewWindw")]
        public static void ShowExample()
        {
            GraphViewWindw wnd = GetWindow<GraphViewWindw>();
            wnd.titleContent = new GUIContent("GraphViewWindw");
        }

        private SceneGraphView _sceneGraphView;
        private InspectorGraphView _inspectorGraphView;
        
        public void CreateGUI()
        {
            // Each editor window contains a root VisualElement object
            VisualElement root = rootVisualElement;
            // Import UXML  "Assets/Editor/Windows/GraphViewWindw.uxml"
            var visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(AssetDatabase.GUIDToAssetPath("f6c584c4055e99e4d89b4a721023b314"));
            visualTree.CloneTree(root);
            // Import USS   "Assets/Editor/Windows/GraphViewWindw.uss"
            var styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>(AssetDatabase.GUIDToAssetPath("4b2fe49cd955a744fa3ea4bb5cdfe19d"));
            root.styleSheets.Add(styleSheet);

            _sceneGraphView = root.Q<SceneGraphView>("SceneGraphView");
            _sceneGraphView.onNodeSelected += OnNodeSelected;
            _sceneGraphView.window = this;

            _inspectorGraphView = root.Q<InspectorGraphView>("InspectorGraphView");
            
            Button addGround = root.Q<Button>("BtnAddGround");
            addGround.clicked += BtnAddGround_OnClick;
        }


        private void BtnAddGround_OnClick()
        {
            _sceneGraphView.CreateNode<GroundNode>(Vector2.zero);
        }

        private void OnNodeSelected(BaseNode node, bool select)
        {
            _inspectorGraphView.UpdateSelection(node, select);
        }
    }
}