using GraphEditor.LevelTrigger;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;


namespace LevelEditorTools.GraphViews
{
    public class HierarchyTriggerView : GraphView
    {
        public new class UxmlFactory : UxmlFactory<HierarchyTriggerView, UxmlTraits>
        {
        }

        public LevelTriggerWindow window;

        public HierarchyTriggerView()
        {
            // Inport USS
            var styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>(AssetDatabase.GUIDToAssetPath("8c54486641f61174e8fe05e59936b0f3"));
            styleSheets.Add(styleSheet);
        }

        
        public void OnHierarchyGUI(bool ongui)
        {
            this.Clear();
            ScrollView scrollView = new ScrollView();
            IMGUIContainer container = new IMGUIContainer(() =>
            {
                
            });
            scrollView.Add(container);
            this.Add(scrollView);
        }
    }
}