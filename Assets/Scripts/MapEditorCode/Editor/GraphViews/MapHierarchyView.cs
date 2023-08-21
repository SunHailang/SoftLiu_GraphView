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

        public MapHierarchyView()
        {
            // Inport USS
            var styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>(AssetDatabase.GUIDToAssetPath("3809fd0b7d9d4ab495256122da5f0407"));
            styleSheets.Add(styleSheet);
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
            this.Add(foldout);
            
            _groupList[index] = foldout;
        }
    }
}