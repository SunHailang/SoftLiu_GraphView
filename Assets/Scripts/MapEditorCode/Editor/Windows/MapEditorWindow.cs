using EditorUtils;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.UIElements;


namespace MapEditor
{
    public class MapEditorWindow : EditorWindow
    {
        [MenuItem("Tools/MapEditorWindow")]
        public static void ShowExample()
        {
            var wnd = GetWindow<MapEditorWindow>();
            wnd.minSize = new Vector2(1350, 650);
            wnd.titleContent = new GUIContent("MapEditorWindow", EditorGUIUtility.IconContent("_Popup@2x").image);
        }

        private void ShowButton(Rect rect)
        {
            var content = EditorGUIUtility.IconContent("cs Script Icon");
            rect.size = new Vector2(20f, 20f);
            rect.position += new Vector2(-2, -2);
            content.tooltip = "Editor Script";
            if (UnityEngine.GUI.Button(rect, content, UnityEngine.GUI.skin.box))
            {
                GraphViewUtils.OpenCodeEditor("LevelTriggerWindow");
            }
        }

        private MapHierarchyView _mapHierarchyView;
        private MapSceneView _mapSceneView;
        private IMGUIContainer _sceneContainer;

        private MapInspectorView _mapInspectorView;

        public void CreateGUI()
        {
            // Each editor window contains a root VisualElement object
            var root = rootVisualElement;

            // Import UXML "Assets/Scripts/MapEditorCode/Editor/Windows/MapEditorWindow.uxml"
            var visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(AssetDatabase.GUIDToAssetPath("8a048fc71575336489859dce45aebc98"));
            visualTree.CloneTree(root);

            // A stylesheet can be added to a VisualElement.
            // The style will be applied to the VisualElement and all of its children.
            // "Assets/Scripts/MapEditorCode/Editor/Windows/MapEditorWindow.uss"
            var styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>(AssetDatabase.GUIDToAssetPath("9a922be7ffa758a4d9b7ee8bdc9f0e43"));
            root.styleSheets.Add(styleSheet);

            #region Toolbar面板

            var toolbarMenuFile = root.Q<ToolbarMenu>("ToolbarMenuFile");
            toolbarMenuFile.menu.AppendAction("New File", NewFileAction);
            toolbarMenuFile.menu.AppendAction("Save", SaveAction);
            toolbarMenuFile.menu.AppendAction("Save As...", SaveAsAction);

            var toolbarMenuEditor = root.Q<ToolbarMenu>("ToolbarMenuEditor");

            var toolbarMenuHelp = root.Q<ToolbarMenu>("ToolbarMenuHelp");
            toolbarMenuHelp.menu.AppendAction("Documentation", DocumentationAction);
            toolbarMenuHelp.menu.AppendAction("About", AboutAction);

            #endregion

            #region 面板分区

            var leftView = root.Q<VisualElement>("LeftView");
            var rightContentView = root.Q<VisualElement>("RightContentView");

            var splitView = new TwoPaneSplitView(0, 300, TwoPaneSplitViewOrientation.Horizontal)
            {
                fixedPaneInitialDimension = 270,
            };
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

            var containerFile = root.Q<IMGUIContainer>("ContainerFile");
            containerFile.onGUIHandler = ContainerFileHandler;

            #region MapHierarchyView

            _mapHierarchyView = root.Q<MapHierarchyView>("MapHierarchyView");
            var scroll = root.Q<ScrollView>("HierarchyScrollView");
            _mapHierarchyView.SetScrollView(scroll);
            var btnAddHierarchyGroup = root.Q<Button>("BtnAddHierarchyGroup");
            btnAddHierarchyGroup.clicked += BtnAddHierarchyGroup_OnClick;

            #endregion

            #region MapSceneView

            _mapSceneView = root.Q<MapSceneView>("MapSceneView");
            var sceneScroll = root.Q<ScrollView>("SceneScrollView");
            _sceneContainer = root.Q<IMGUIContainer>("SceneContainer");
            _sceneContainer.onGUIHandler = SceneContainerHandler;

            var btnLeft = root.Q<Button>("BtnAddLeftLine");
            btnLeft.clicked += BtnAddLeftLine_OnClick;
            var btnReduceLeftLine = root.Q<Button>("BtnReduceLeftLine");
            btnReduceLeftLine.clicked += BtnReduceLeftLine_OnClick;

            var btnUp = root.Q<Button>("BtnAddTopLine");
            btnUp.clicked += BtnAddTopLine_OnClick;
            var btnReduceTopLine = root.Q<Button>("BtnReduceTopLine");
            btnReduceTopLine.clicked += BtnReduceTopLine_OnClick;

            var btnRight = root.Q<Button>("BtnAddRightLine");
            btnRight.clicked += BtnAddRightLine_OnClick;
            var btnReduceRightLine = root.Q<Button>("BtnReduceRightLine");
            btnReduceRightLine.clicked += BtnReduceRightLine_OnClick;

            var btnBottom = root.Q<Button>("BtnAddBottomLine");
            btnBottom.clicked += BtnAddBottomLine_OnClick;
            var btnReduceBottomLine = root.Q<Button>("BtnReduceBottomLine");
            btnReduceBottomLine.clicked += BtnReduceBottomLine_OnClick;

            _mapSceneView.InitDrawParentElement(_sceneContainer);

            #endregion
        }

        

        private void ContainerFileHandler()
        {
        }

        #region Toolbar 面板事件

        private void NewFileAction(DropdownMenuAction obj)
        {
        }

        private void SaveAction(DropdownMenuAction obj)
        {
        }

        private void SaveAsAction(DropdownMenuAction obj)
        {
        }

        private void DocumentationAction(DropdownMenuAction obj)
        {
        }

        private void AboutAction(DropdownMenuAction obj)
        {
        }

        #endregion

        #region Hierarchy 面板事件

        private void BtnAddHierarchyGroup_OnClick()
        {
            _mapHierarchyView.BtnAddHierarchyGroup_OnClick();
        }

        #endregion

        #region Scene 面板事件

        private void SceneContainerHandler()
        {
            _mapSceneView.SceneContainerHandler();
        }

        private void BtnAddLeftLine_OnClick()
        {
            _mapSceneView.BtnLeftLine_OnClick();
        }

        private void BtnAddTopLine_OnClick()
        {
            _mapSceneView.BtnTopLine_OnClick();
        }

        private void BtnAddRightLine_OnClick()
        {
            _mapSceneView.BtnRightLine_OnClick();
        }

        private void BtnAddBottomLine_OnClick()
        {
            _mapSceneView.BtnBottomLine_OnClick();
        }
        
        private void BtnReduceLeftLine_OnClick()
        {
            _mapSceneView.BtnLeftLine_OnClick(false);
        }

        private void BtnReduceTopLine_OnClick()
        {
            _mapSceneView.BtnTopLine_OnClick(false);
        }

        private void BtnReduceRightLine_OnClick()
        {
            _mapSceneView.BtnRightLine_OnClick(false);
        }

        private void BtnReduceBottomLine_OnClick()
        {
            _mapSceneView.BtnBottomLine_OnClick(false);
        }

        #endregion
    }
}