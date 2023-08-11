using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.UIElements;


public class TestWindow : EditorWindow
{
    [MenuItem("Tools/TestWindow _a")]
    public static void ShowExample()
    {
        TestWindow wnd = GetWindow<TestWindow>();
        wnd.titleContent = new GUIContent("TestWindow");
    }

    public void CreateGUI()
    {
        // Each editor window contains a root VisualElement object
        var root = rootVisualElement;

        // Import UXML
        var visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/Scripts/LevelEditorTools/Editor/TestWindow/TestWindow.uxml");
        root.Add(visualTree.CloneTree());

        // A stylesheet can be added to a VisualElement.
        // The style will be applied to the VisualElement and all of its children.
        var styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>("Assets/Scripts/LevelEditorTools/Editor/TestWindow/TestWindow.uss");
        root.styleSheets.Add(styleSheet);
        var panelIndex = 0;
        var panelWidth = 200;
        var panelView = new TwoPaneSplitView(panelIndex, panelWidth, TwoPaneSplitViewOrientation.Horizontal);
        root.Add(panelView);
        var leftView = root.Q<VisualElement>("LeftElement");
        panelView.Add(leftView);
        var rightView = root.Q<VisualElement>("RightElement");
        panelView.Add(rightView);
        
        //panelIndex++;
        var panelView1 = new TwoPaneSplitView(panelIndex, panelWidth, TwoPaneSplitViewOrientation.Horizontal);
        rightView.Add(panelView1);
        var leftView1 = root.Q<VisualElement>("LeftElement1");
        panelView1.Add(leftView1);
        var rightView1 = root.Q<VisualElement>("RightElement1");
        panelView1.Add(rightView1);
        
        //panelIndex++;
        var panelView2 = new TwoPaneSplitView(panelIndex, panelWidth, TwoPaneSplitViewOrientation.Horizontal);
        rightView1.Add(panelView2);
        var leftView2 = root.Q<VisualElement>("LeftElement2");
        panelView2.Add(leftView2);
        var rightView2 = root.Q<VisualElement>("RightElement2");
        panelView2.Add(rightView2);

    }
}