using LevelEditorTools;
using LevelEditorTools.GraphViews;
using LevelEditorTools.Save;
using LevelEditorTools.Editor.Nodes;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace GraphEditor.GraphViews
{
    public class GraphViewWindw : EditorWindow
    {
        [MenuItem("Tools/GraphViewWindw")]
        public static void ShowExample()
        {
            GraphViewWindw wnd = GetWindow<GraphViewWindw>();
            wnd.titleContent = new GUIContent("GraphViewWindw");
            wnd.minSize = new Vector2(400, 280);
        }

        [UnityEditor.Callbacks.OnOpenAsset(1)]
        public static bool OpenGraphViewWindow(int instanceID, int line)
        {
            Object activeObject = EditorUtility.InstanceIDToObject(instanceID);
            string path = AssetDatabase.GetAssetPath(instanceID);
            if (activeObject is SceneContainer container)
            {
                GraphViewWindw wnd = GetWindow<GraphViewWindw>();
                wnd.titleContent = new GUIContent("GraphViewWindw");
                wnd.LoadSceneView(path, container);
                return true;
            }

            return false;
        }

        private TextField _inputPath = null;
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

            var splitView = new TwoPaneSplitView(0, 300, TwoPaneSplitViewOrientation.Horizontal);
            var leftView = root.Q<VisualElement>("LeftView");
            splitView.Add(leftView);
            var rightView = root.Q<VisualElement>("RightView");
            splitView.Add(rightView);
            root.Add(splitView);

            _inputPath = root.Q<TextField>("InputPath");
            
            Button save = root.Q<Button>("SaveScene");
            save.clicked += BtnSaveScene_OnClick;


            _sceneGraphView = root.Q<SceneGraphView>("SceneGraphView");
            _sceneGraphView.onNodeSelected += OnNodeSelected;
            _sceneGraphView.window = this;

            _inspectorGraphView = root.Q<InspectorGraphView>("InspectorGraphView");
        }

        private void LoadSceneView(string path, SceneContainer container)
        {
            _inputPath.value = path;
            GraphSceneSaveUtility.GetInstance(_sceneGraphView).Load(container);
        }

        private void BtnSaveScene_OnClick()
        {
            _inputPath.value = GraphSceneSaveUtility.GetInstance(_sceneGraphView).Save(_inputPath.value);
        }

        private void OnNodeSelected(BaseNode node, bool select)
        {
            _inspectorGraphView.OnInspectorGUI(node, select);
        }

    }
}