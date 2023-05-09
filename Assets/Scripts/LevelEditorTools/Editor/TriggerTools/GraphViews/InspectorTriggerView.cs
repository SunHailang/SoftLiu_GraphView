using GraphEditor.GraphViews;
using GraphEditor.LevelTrigger;
using LevelEditorTools.Editor.Nodes;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;


namespace LevelEditorTools.GraphViews
{
    public class InspectorTriggerView : GraphView
    {
        public new class UxmlFactory : UxmlFactory<InspectorTriggerView, UxmlTraits>
        {
        }

        public LevelTriggerWindow window;

        public InspectorTriggerView()
        {
            // Inport USS
            var styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>(AssetDatabase.GUIDToAssetPath("8c54486641f61174e8fe05e59936b0f3"));
            styleSheets.Add(styleSheet);
        }


        public void OnInspectorGUI(BaseNode nodeView, bool selected)
        {
            this.Clear();
            if (!selected)
            {
                return;
            }

            if (nodeView == null || nodeView.State == null)
            {
                return;
            }

            ScrollView scrollView = new ScrollView();
            IMGUIContainer container = new IMGUIContainer(() =>
            {
                bool hasChange = nodeView.DrawInspectorGUI();
                if (hasChange && !window.hasUnsavedChanges)
                {
                    window.SetUnsaveChange(true);
                }
            });
            scrollView.Add(container);
            this.Add(scrollView);
        }
    }
}