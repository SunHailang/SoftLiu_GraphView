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
        

        public void OnInspectorGUI(bool ongui)
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