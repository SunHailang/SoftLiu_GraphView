using EditorUtils;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.UIElements;


public class MapEditorWindow : EditorWindow
{
    [MenuItem("Tools/MapEditorWindow")]
    public static void ShowExample()
    {
        var wnd = GetWindow<MapEditorWindow>();
        wnd.titleContent = new GUIContent("MapEditorWindow");
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
    
    public void CreateGUI()
    {
        // Each editor window contains a root VisualElement object
        VisualElement root = rootVisualElement;

        // Import UXML "Assets/Scripts/MapEditorCode/Editor/Windows/MapEditorWindow.uxml"
        var visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(AssetDatabase.GUIDToAssetPath("8a048fc71575336489859dce45aebc98"));
        visualTree.CloneTree(root);

        // A stylesheet can be added to a VisualElement.
        // The style will be applied to the VisualElement and all of its children.
        // "Assets/Scripts/MapEditorCode/Editor/Windows/MapEditorWindow.uss"
        var styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>(AssetDatabase.GUIDToAssetPath("9a922be7ffa758a4d9b7ee8bdc9f0e43"));
        root.styleSheets.Add(styleSheet);
    }
}