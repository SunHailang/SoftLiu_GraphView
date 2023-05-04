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

        public InspectorTriggerView()
        {
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
            IMGUIContainer container = new IMGUIContainer(nodeView.DrawInspectorGUI);
            scrollView.Add(container);
            this.Add(scrollView);
        }
    }
}