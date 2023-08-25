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
        public Color ColorObj;
    }

    public class FoldoutList : VisualElement
    {
        public class Factory : UxmlFactory<FoldoutList, UxmlTraits>
        {
        }

        private const string _defaultLabel = "Select Item";

        private Foldout _foldout;
        private ListView _listView;

        private List<FoldoutListItem> _items = new List<FoldoutListItem>();

        private TextField _curTextField = null;
        private string _curGroupName = "新建组";

        private int _curUnique = -1;

        private System.Action<int> _removeCallback;

        public event System.Action<FoldoutListItem> OnSelectListItem; 

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
            RegisterCallback<KeyDownEvent>(OnKeyDownEvent);
        }

        private void OnKeyDownEvent(KeyDownEvent evt)
        {
            if (evt.keyCode == KeyCode.Escape)
            {
                _removeCallback?.Invoke(_curUnique);
            }
        }

        public void AddItem(string assetPath, Color color)
        {
            var go = AssetDatabase.LoadAssetAtPath<GameObject>(assetPath);
            var item = new FoldoutListItem();
            item.SetValue(go, color);
            _items.Add(item);
            _listView.Rebuild();
        }

        public void SetUnique(int index)
        {
            _curUnique = index;
        }

        public void SetGroupName(string group)
        {
            _foldout.text = group;
            _curGroupName = group;
        }

        public void SetRemoveCallback(System.Action<int> callback)
        {
            _removeCallback = callback;
        }

        public int GetUnique()
        {
            return _curUnique;
        }
        
        public string GetGroupName()
        {
            return _curGroupName;
        }

        public List<FoldoutListItem> GetItemList()
        {
            return _items;
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
            _items.Add(new FoldoutListItem());
            _listView.Rebuild();
        }

        private void OnListViewSelectionChanged(IEnumerable<object> obj)
        {
            // _listView.selectedIndex;
            Debug.Log($"FoldoutList Select: {_curGroupName}, {_listView.selectedIndex}");
            var item = _items[_listView.selectedIndex];
            OnSelectListItem?.Invoke(item);
        }

        private void RemoveItem(FoldoutListItem item)
        {
            if (_items.Remove(item))
            {
                _listView.Rebuild();
            }
        }
        
        private VisualElement OnListViewMakItem()
        {
            var item = new FoldoutListItem();
            item.SetRemoveCallback(RemoveItem);
            return item;
        }

        private void OnListViewBindItem(VisualElement arg1, int arg2)
        {
            if (arg1 is FoldoutListItem item)
            {
                var temp = _items[arg2];
                if (temp == null)
                {
                    _items[arg2] = item;
                }
                else
                {
                    _items[arg2] = item;
                    item.SetValue(temp.GetFieldObj(), temp.GetColor());
                }
            }
        }
    }
}