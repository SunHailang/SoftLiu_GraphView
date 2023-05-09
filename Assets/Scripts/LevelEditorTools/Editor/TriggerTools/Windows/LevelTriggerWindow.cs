using System.IO;
using LevelEditorTools.Editor.Nodes;
using LevelEditorTools.GraphViews;
using LevelEditorTools.Save;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;


namespace GraphEditor.LevelTrigger
{
    public class LevelTriggerWindow : EditorWindow
    {
        [MenuItem("Tools/LevelTriggerWindow")]
        public static void ShowExample()
        {
            LevelTriggerWindow wnd = GetWindow<LevelTriggerWindow>();
            wnd.titleContent = new GUIContent("关卡触发器");
            wnd.minSize = new Vector2(600, 300);
        }
        
        [UnityEditor.Callbacks.OnOpenAsset(1)]
        public static bool OpenGraphViewWindow(int instanceID, int line)
        {
            Object activeObject = EditorUtility.InstanceIDToObject(instanceID);
            string path = AssetDatabase.GetAssetPath(instanceID);
            if (activeObject is LevelTriggerContainer container)
            {
                LevelTriggerWindow wnd = GetWindow<LevelTriggerWindow>();
                wnd.titleContent = new GUIContent("关卡触发器");
                wnd.LoadSceneView(path, container);
                return true;
            }

            return false;
        }
        
        private SceneTriggerView _sceneTrigger;
        private InspectorTriggerView _inspectorTrigger;
        private HierarchyTriggerView _hierarchyTrigger;

        private TextField _textField;

        public void CreateGUI()
        {
            // Each editor window contains a root VisualElement object
            VisualElement root = rootVisualElement;
            // Import UXML    LevelTriggerWindow.uxml
            var visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(AssetDatabase.GUIDToAssetPath("a1f98bbc52eff2c4c89a3bdc204df4c3"));
            visualTree.CloneTree(root);

            // Import USS  LevelTriggerWindow.uss
            var styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>(AssetDatabase.GUIDToAssetPath("4b2fe49cd955a744fa3ea4bb5cdfe19d"));
            root.styleSheets.Add(styleSheet);

            #region 面板分区

            var leftView = root.Q<VisualElement>("LeftView");
            var rightContentView = root.Q<VisualElement>("RightContentView");

            var splitView = new TwoPaneSplitView(0, 300, TwoPaneSplitViewOrientation.Horizontal);
            splitView.fixedPaneInitialDimension = 200;
            root.Add(splitView);
            splitView.Add(leftView);
            splitView.Add(rightContentView);

            var midView = root.Q<VisualElement>("MidView");
            var rightView = root.Q<VisualElement>("RightView");

            var splitRightCountView = new TwoPaneSplitView(1, 300, TwoPaneSplitViewOrientation.Horizontal);
            rightContentView.Add(splitRightCountView);
            splitRightCountView.Add(midView);
            splitRightCountView.Add(rightView);

            #endregion

            _textField = root.Q<TextField>("InputPath");
            
            Button btnSave = root.Q<Button>("BtnSave");
            btnSave.clicked += BtnSave_OnClick;
            
            _sceneTrigger = root.Q<SceneTriggerView>("SceneTriggerView");
            _sceneTrigger.window = this;
            _sceneTrigger.onNodeSelected += OnNodeSelected;
            
            _inspectorTrigger = root.Q<InspectorTriggerView>("InspectorTriggerView");
            _inspectorTrigger.window = this;
            _hierarchyTrigger = root.Q<HierarchyTriggerView>("HierarchyTriggerView");
            _hierarchyTrigger.window = this;
        }
        
        public void SetUnsaveChange(bool change)
        {
            hasUnsavedChanges = change;
        }

        public override void SaveChanges()
        {
            LevelTriggerSaveUtility.GetInstance(_sceneTrigger).Save(_textField.value);
            base.SaveChanges();
        }

        private void LoadSceneView(string path, LevelTriggerContainer container)
        {
            _textField.value = path;
            LevelTriggerSaveUtility.GetInstance(_sceneTrigger).Load(container);
        }

        private void BtnSave_OnClick()
        {
            _textField.value = LevelTriggerSaveUtility.GetInstance(_sceneTrigger).Save(_textField.value);
            if (File.Exists(_textField.value))
            {
                SetUnsaveChange(false);
            }
        }
        
        private void OnNodeSelected(BaseNode node, bool select)
        {
            _inspectorTrigger.OnInspectorGUI(node, select);
        }
    }
}