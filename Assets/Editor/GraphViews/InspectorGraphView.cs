using Editor.Nodes;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace Editor.GraphViews
{
    public class InspectorGraphView : GraphView
    {
        public class UxmlFactor : UxmlFactory<InspectorGraphView, UxmlTraits>
        {
        }

        private UnityEditor.Editor _editor;

        public InspectorGraphView()
        {
            // Inport USS
            var styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>(AssetDatabase.GUIDToAssetPath("8c54486641f61174e8fe05e59936b0f3"));
            styleSheets.Add(styleSheet);
        }
        
        public void UpdateSelection(BaseNode nodeView, bool selected)
        {
            Clear();
            Debug.Log($"显示节点的Inspector面板 {selected}");
            UnityEngine.Object.DestroyImmediate(_editor);

            if (nodeView == null || nodeView.State == null)
                return;
            _editor = UnityEditor.Editor.CreateEditor(nodeView.State);
            ScrollView scrollView = new ScrollView();
            IMGUIContainer container = new IMGUIContainer(() => {
                if (nodeView != null)
                {
                    //EditorGUILayout.LabelField("Node名称：", nodeView.title);
                    _editor.OnInspectorGUI();
                }
            });
            scrollView.Add(container);
            this.Add(scrollView);
        }
    }
}
