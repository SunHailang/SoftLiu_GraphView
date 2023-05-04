using LevelEditorTools.Editor.Nodes;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine.UIElements;

namespace LevelEditorTools.GraphViews
{
    public class InspectorGraphView : GraphView
    {
        public new class UxmlFactory: UxmlFactory<InspectorGraphView, UxmlTraits>
        {
        }

        public InspectorGraphView()
        {
            // Inport USS
            var styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>(AssetDatabase.GUIDToAssetPath("8c54486641f61174e8fe05e59936b0f3"));
            styleSheets.Add(styleSheet);
        }
        
        public void UpdateSelection(BaseNode nodeView, bool selected)
        {
            Clear();
            if (!selected)
            {
                return;
            }
            if (nodeView == null || nodeView.State == null)
                return;
            ScrollView scrollView = new ScrollView();
            IMGUIContainer container = new IMGUIContainer(() =>
            {
                nodeView.DrawInspectorGUI();
            });
            scrollView.Add(container);
            this.Add(scrollView);
        }
    }
}
