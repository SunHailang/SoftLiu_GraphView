using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace MapEditor
{
    public class ResData
    {
        public string Key;
        public string ResPath;
        public Color Color = Color.black;
    }

    public class ResGroupData
    {
        public string Key;
        public List<ResData> Data = new List<ResData>();
    }

    public class MapHierarchyView : GraphView
    {
        public new class UxmlFactory : UxmlFactory<MapHierarchyView, UxmlTraits>
        {
        }

        private readonly Dictionary<int, FoldoutList> _groupList = new Dictionary<int, FoldoutList>();

        private ScrollView _scrollView;

        public MapHierarchyView()
        {
            // Inport USS
            var styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>(AssetDatabase.GUIDToAssetPath("3809fd0b7d9d4ab495256122da5f0407"));
            styleSheets.Add(styleSheet);
        }

        public void SetScrollView(ScrollView scrollView)
        {
            _scrollView = scrollView;
        }

        public void BtnAddHierarchyGroup_OnClick()
        {
            var index = _groupList.Count;
            while (_groupList.ContainsKey(index))
            {
                index++;
            }
            var foldout = new FoldoutList();
            foldout.SetUnique(index);
            foldout.SetRemoveCallback(RemoveCallback);
            _scrollView.Add(foldout);
            
            _groupList[index] = foldout;
        }

        private void RemoveCallback(int unique)
        {
            if (_groupList.TryGetValue(unique, out var foldout))
            {
                _scrollView.Remove(foldout);
                _groupList.Remove(unique);
            }
        }
    }
}