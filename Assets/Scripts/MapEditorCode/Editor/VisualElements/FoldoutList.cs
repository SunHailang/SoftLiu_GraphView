using System.Collections.Generic;
using Unity.Entities.UniversalDelegates;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.UIElements;

namespace MapEditor
{
    public class ObjectData
    {
        public int Index;
        public string Label;
        public GameObject FieldObj;
    }

    public class FoldoutList : VisualElement
    {
        public class Factory : UxmlFactory<FoldoutList, UxmlTraits>
        {
        }

        private const string _defaultLabel = "Select Item";

        private Foldout _foldout;
        private ListView _listView;

        private List<ObjectData> _items = new List<ObjectData>();

        private int _curSelectIndex = 0;
        private int _curRefreshIndex = 0;

        public FoldoutList()
        {
            // guid 
            // Import UXML // "Assets/Scripts/MapEditorCode/Editor/VisualElements/FoldoutList.uxml"
            var visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(AssetDatabase.GUIDToAssetPath("29bf991b4331e1742bdb2ceb51f6fa18"));
            visualTree.CloneTree(this);

            _foldout = this.Q<Foldout>("Foldout");
            _foldout.value = true;

            var btn = this.Q<Button>("BtnAddItem");
            btn.clicked += BtnAddItem_OnClick;

            _listView = new ListView(_items, 20, OnListViewMakItem, OnListViewBindItem)
            {
                style =
                {
                    position = Position.Relative,
                    right = 0,
                    left = -30,
                    marginLeft = 32
                }
            };
            _listView.onSelectionChange += OnListViewSelectionChanged;
            _foldout.Add(_listView);
        }

        private void BtnAddItem_OnClick()
        {
            _items.Add(new ObjectData
            {
                Index = _items.Count,
                Label = _defaultLabel,
                FieldObj = null,
            });
            _curRefreshIndex = 0;
            _listView.Rebuild();
        }

        private void OnListViewSelectionChanged(IEnumerable<object> obj)
        {
            _curSelectIndex = _listView.selectedIndex;
        }

        private VisualElement OnListViewMakItem()
        {
            var objField = new ObjectField
            {
                objectType = typeof(GameObject),
                allowSceneObjects = false,
                style =
                {
                    position = Position.Relative,
                    marginLeft = 0, 
                    left = 0,
                    right = 0
                }
            };
            objField.tabIndex = _curRefreshIndex;
            _curRefreshIndex++;
            objField.RegisterValueChangedCallback(v =>
            {
                var go = v.newValue as GameObject;
                objField.value = go;
                _items[objField.tabIndex].FieldObj = go;
                var label = go == null ? _defaultLabel : go.name;
                objField.label = label;
                _items[objField.tabIndex].Label = label;
            });

            return objField;
        }

        private void OnListViewBindItem(VisualElement arg1, int arg2)
        {
            if (arg1 is ObjectField obj)
            {
                if (_items[arg2].FieldObj != null)
                {
                    _items[arg2].Label = _items[arg2].FieldObj.name;
                }
                obj.label = _items[arg2].Label;
                obj.SetValueWithoutNotify(_items[arg2].FieldObj);
            }
        }
    }
}