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
        }

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

            var leftView = root.Q<VisualElement>("LeftView");
            var rightContentView = root.Q<VisualElement>("RightContentView");

            var splitView = new TwoPaneSplitView(0, 300, TwoPaneSplitViewOrientation.Horizontal);
            root.Add(splitView);
            splitView.Add(leftView);
            splitView.Add(rightContentView);
            
            var midView = root.Q<VisualElement>("MidView");
            var rightView = root.Q<VisualElement>("RightView"); 
            
            var splitRightCountView = new TwoPaneSplitView(0, 300, TwoPaneSplitViewOrientation.Horizontal);
            rightContentView.Add(splitRightCountView);
            splitRightCountView.Add(midView);
            splitRightCountView.Add(rightView);
        }
    }
}