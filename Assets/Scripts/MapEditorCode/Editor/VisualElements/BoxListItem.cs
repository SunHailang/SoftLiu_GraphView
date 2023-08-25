using Unity.EditorCoroutines.Editor;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.UIElements;


namespace MapEditor
{
    public class BoxListItem : VisualElement
    {
        public class Factory : UxmlFactory<BoxListItem, UxmlTraits>
        {
        }

        private ObjectField _objectField;
        private ColorField _colorField;
        private Image _previewImage;

        public BoxListItem()
        {
            // Import UXML Assets/Scripts/MapEditorCode/Editor/VisualElements/BoxListItem.uxml
            var visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(AssetDatabase.GUIDToAssetPath("0579da2e0fa89b8449c4a5ca1d0f4e24"));
            visualTree.CloneTree(this);

            _objectField = this.Q<ObjectField>("ObjectField");
            _objectField.objectType = typeof(GameObject);
            
            _colorField = this.Q<ColorField>("ColorField");
            _previewImage = this.Q<Image>("PreviewImage");
        }

        public void SetObjectField(string path)
        {
            var obj = AssetDatabase.LoadAssetAtPath<GameObject>(path);
            _objectField.value = obj;
            _objectField.label = obj == null ? "" : obj.name;

            MapEditorWindow.MapWindow.StartCoroutine(LoadPreviewTexture());
        }

        public void SetColorField(Color color)
        {
            _colorField.value = color;
        }


        private System.Collections.IEnumerator LoadPreviewTexture()
        {
            Texture2D tex2D = null;
            if (_objectField.value != null)
            {
                do
                {
                    tex2D = AssetPreview.GetAssetPreview(_objectField.value);
                    yield return null;

                } while (tex2D == null);
            }

            _previewImage.image = tex2D;
        }
    }
}