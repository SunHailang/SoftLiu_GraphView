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
            
            // 滚轮
            // this.SetupZoom(ContentZoomer.DefaultMinScale, ContentZoomer.DefaultMaxScale);
            this.SetupZoom(0.2f, 2.0f);
            // 可以选中单个Node
            this.AddManipulator(new SelectionDragger());
            // 按住 Alt键 可以拖动
            this.AddManipulator(new ContentDragger());
            // 可以框选多个Node
            this.AddManipulator(new RectangleSelector());
            
            string path = AssetDatabase.GUIDToAssetPath("73411fe8094701f49b6a65893deb79fa");
            StyleSheet styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>(path);
            this.styleSheets.Add(styleSheet);
        }
        
    }
}