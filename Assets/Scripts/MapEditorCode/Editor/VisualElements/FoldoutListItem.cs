using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.UIElements;

namespace MapEditor
{
    public class FoldoutListItem : VisualElement
    {
        public class Factory : UxmlFactory<FoldoutListItem, UxmlTraits>
        {
        }

        private ObjectField _objectField;
        private ColorField _colorField;

        private System.Action<FoldoutListItem> _removeCallback;

        public FoldoutListItem()
        {
            // Import UXML "Assets/Scripts/MapEditorCode/Editor/VisualElements/FoldoutListItem.uxml"
            var visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(AssetDatabase.GUIDToAssetPath("6fcaa547c00120e4e8301ec545dcff80"));
            visualTree.CloneTree(this);

            _colorField = this.Q<ColorField>("ColorField");
            _colorField.value = UnityEngine.Random.ColorHSV(0.2f, 1.0f);

            _objectField = this.Q<ObjectField>("ObjectField");
            _objectField.objectType = typeof(GameObject);
            _objectField.RegisterValueChangedCallback(v =>
            {
                var go = v.newValue as GameObject;

                OnObjectFieldValueChanged(go);
            });
            RegisterCallback<MouseEnterEvent>(OnMouseEnterEvent);
            RegisterCallback<MouseLeaveEvent>(OnMouseLevelEvent);
            
            RegisterCallback<KeyDownEvent>(OnKeyDownEvent);
        }

        public void SetRemoveCallback(System.Action<FoldoutListItem> callback)
        {
            _removeCallback = callback;
        }
        
        private void OnKeyDownEvent(KeyDownEvent evt)
        {
            if (evt.keyCode == KeyCode.Escape)
            {
                _removeCallback?.Invoke(this);
            }
        }

        private void OnMouseEnterEvent(MouseEnterEvent evt)
        {
            if (_objectField.value != null)
            {
            }
        }

        private void OnMouseLevelEvent(MouseLeaveEvent evt)
        {
        }

        private void OnObjectFieldValueChanged(GameObject go)
        {
            _objectField.value = go;
            _objectField.label = go == null ? "New Item" : go.name;
        }

        public void SetValue(GameObject go, Color color)
        {
            OnObjectFieldValueChanged(go);
            _colorField.value = color;
        }

        public string GetLabel()
        {
            return _objectField.value == null ? string.Empty : _objectField.value.name;
        }

        public GameObject GetFieldObj()
        {
            return _objectField.value as GameObject;
        }

        public string GetObjFieldPath()
        {
            return _objectField.value == null ? string.Empty : AssetDatabase.GetAssetPath(_objectField.value);
        }

        public Color GetColor()
        {
            return _colorField.value;
        }
    }
}