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

        public event System.Action<FoldoutListItem> OnSelectFoldoutItem; 

        public MapHierarchyView()
        {
            // Inport USS
            var styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>(AssetDatabase.GUIDToAssetPath("3809fd0b7d9d4ab495256122da5f0407"));
            styleSheets.Add(styleSheet);
        }

        public void Initialization(MapResData data)
        {
            _groupList.Clear();
            _scrollView.Clear();
            
            data = AssetDatabase.LoadAssetAtPath<MapResData>("Assets/ClashofClans/Resources/MapResDataContainer.asset");
            foreach (var itemData in data.MapItemList)
            {
                var unique = itemData.ItemGroupUnique;
                var group = itemData.ItemGroup;
                if (!_groupList.TryGetValue(unique, out var foldoutList))
                {
                    foldoutList = AddFoldoutList(unique, group);
                }
                foldoutList.AddItem(itemData.ItemPath, itemData.ItemColor);
            }
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

            AddFoldoutList(index, "新建组");
        }

        private FoldoutList AddFoldoutList(int unique, string group)
        {
            var foldout = new FoldoutList();
            foldout.SetUnique(unique);
            foldout.SetGroupName(group);
            foldout.SetRemoveCallback(RemoveCallback);
            foldout.OnSelectListItem += OnSelectItem;
            _scrollView.Add(foldout);
            _groupList[unique] = foldout;

            return foldout;
        }

        private void OnSelectItem(FoldoutListItem item)
        {
            OnSelectFoldoutItem?.Invoke(item);
        }

        private void RemoveCallback(int unique)
        {
            if (_groupList.TryGetValue(unique, out var foldout))
            {
                _scrollView.Remove(foldout);
                _groupList.Remove(unique);
            }
        }

        public void Save()
        {
            var container = ScriptableObject.CreateInstance<MapResData>();
            foreach (var (_, value) in _groupList)
            {
                var group = value.GetGroupName();
                var unique = value.GetUnique();
                foreach (var item in value.GetItemList())
                {
                    var itemData = new MapItemData
                    {
                        ItemGroupUnique = unique,
                        ItemGroup = group,
                        ItemColor = item.GetColor(),
                        ItemPath = item.GetObjFieldPath()
                    };
                    container.MapItemList.Add(itemData);
                }
            }

            var filePath = EditorUtility.SaveFilePanel("选择文件", Application.dataPath, "MapResDataContainer", "asset");
            if (!string.IsNullOrEmpty(filePath))
            {
                var path = filePath.Substring(Application.dataPath.Length - 6);
                AssetDatabase.CreateAsset(container, path);
            }

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }
    }
}