using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.UIElements;


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
            var styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>(AssetDatabase.GUIDToAssetPath("668fccfc2dcc65c4f875a7a3220c8257"));
            root.styleSheets.Add(styleSheet);
            
            
        }
    }
}