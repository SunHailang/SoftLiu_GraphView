using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine.Pool;
using UnityEngine.UIElements;

namespace MapEditor
{
    public class MapInspectorView : GraphView
    {
        public new class UxmlFactory : UxmlFactory<MapInspectorView, UxmlTraits>
        {
        }


        private ObjectPool<BoxListItem> _objectPool;

        private ScrollView _scrollView;

        public MapInspectorView()
        {
            // Inport USS
            var styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>(AssetDatabase.GUIDToAssetPath("3809fd0b7d9d4ab495256122da5f0407"));
            styleSheets.Add(styleSheet);

            _objectPool = new ObjectPool<BoxListItem>(OnCreateAction, OnGetAction, OnReleaseAction, OnDestroyAction, true, 5, 20);
        }

        private BoxListItem OnCreateAction()
        {
            var item = new BoxListItem();
            return item;
        }

        private void OnGetAction(BoxListItem item)
        {
        }

        private void OnReleaseAction(BoxListItem item)
        {
        }

        private void OnDestroyAction(BoxListItem item)
        {
        }

        public void SetScrollView(ScrollView scrollView)
        {
            _scrollView = scrollView;
        }

        public void OnSelectButtonBox(ButtonBox box)
        {
            for (var i = 0; i < _scrollView.childCount; i++)
            {
                var element = _scrollView.ElementAt(i);
                if (element is BoxListItem item)
                {
                    _objectPool.Release(item);
                }
            }
            _scrollView.Clear();

            var list = box.BoxColorList;
            foreach (var data in list)
            {
                var item = _objectPool.Get();
                item.SetObjectField(data.DataPath);
                item.SetColorField(data.DataColor);

                _scrollView.Add(item);
            }
        }
    }
}