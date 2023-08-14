using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine.UIElements;

namespace MapEditor
{
    public class MapInspectorView : GraphView
    {
        public new class UxmlFactory : UxmlFactory<MapInspectorView, UxmlTraits>
        {
        }
        
        public MapInspectorView()
        {
            // Inport USS
            var styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>(AssetDatabase.GUIDToAssetPath("3809fd0b7d9d4ab495256122da5f0407"));
            styleSheets.Add(styleSheet);
        }
    }
}