using System.Collections.Generic;
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

        private TextField _curTextField = null;
        private string _curGroupName = "新建组";

        private int _curUnique = -1;

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
                style=
                {
                    position = Position.Relative,
                    right = 0,
                    left = -3,
                    marginLeft = 3
                }
            };
            _listView.onSelectionChange += OnListViewSelectionChanged;
            _foldout.Add(_listView);

            RegisterCallback<MouseDownEvent>(OnMouseDownEvent);
        }

        public void SetUnique(int index)
        {
            _curUnique = index;
        }

        private void OnMouseDownEvent(MouseDownEvent evt)
        {
            if (evt.button == 2 && !this.Contains(_curTextField))
            {
                _curTextField ??= new TextField
                {
                    style =
                    {
                        position = Position.Absolute,
                        top = 10,
                        left = 18,
                        right = 28
                    },
                    value = _curGroupName
                };

                this.Add(_curTextField);
            }
            else if (_curTextField != null && this.Contains(_curTextField))
            {
                if (!string.IsNullOrEmpty(_curTextField.value) && _curGroupName != _curTextField.value)
                {
                    _curGroupName = _curTextField.value;
                    _foldout.text = _curGroupName;
                }

                this.Remove(_curTextField);
            }
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
                },
                tabIndex = _curRefreshIndex
            };
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