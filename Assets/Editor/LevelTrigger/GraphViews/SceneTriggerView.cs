using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine.UIElements;

namespace GraphEditor.LevelTrigger
{
    public class SceneTriggerView : GraphView
    {
        public class UxmlFactor : UxmlFactory<SceneTriggerView, UxmlTraits>
        {
        }

        public SceneTriggerView()
        {
            Insert(0, new GridBackground());
            string path = AssetDatabase.GUIDToAssetPath("ac7e11f2e2d017e4c8733bc7346e56af");
            StyleSheet styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>(path);
            this.styleSheets.Add(styleSheet);
        }
        
    }
}