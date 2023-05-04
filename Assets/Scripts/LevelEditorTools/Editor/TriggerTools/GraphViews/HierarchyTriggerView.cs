using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;


namespace GraphEditor.LevelTrigger
{
    public class HierarchyTriggerView : GraphView
    {
        public new class UxmlFactory : UxmlFactory<HierarchyTriggerView, UxmlTraits>
        {
        }


        public HierarchyTriggerView()
        {
            
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