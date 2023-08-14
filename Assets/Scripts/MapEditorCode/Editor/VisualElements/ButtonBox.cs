using System.Collections.Generic;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.UIElements;

namespace MapEditor
{
    public class ButtonBox : VisualElement
    {
        public class Factory : UxmlFactory<ButtonBox, UxmlTraits>
        {
        }

        private VisualElement _background;
        private Label _des;

        private Texture2D _image;

        private const int Width = 50;
        private const int Height = 50;

        private Dictionary<string, Color> _colors = new Dictionary<string, Color>();

        public ButtonBox()
        {
            // Import UXML // "Assets/Scripts/MapEditorCode/Editor/VisualElements/ButtonBox.uxml"
            var visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(AssetDatabase.GUIDToAssetPath("b2c86cd3e11c0cd4c805550134ac4a6b"));
            visualTree.CloneTree(this);
            _background = this.Q<VisualElement>("BtnElement");
            _des = this.Q<Label>("Desc");

            this.style.width = Width;
            this.style.height = Height;

            RegisterCallback<MouseEnterEvent>(OnMouseEnterEvent);
            RegisterCallback<MouseLeaveEvent>(OnMouseLevelEvent);

            RegisterCallback<MouseUpEvent>(OnMouseUpEvent);
            RegisterCallback<MouseDownEvent>(OnMouseDownEvent);

            RegisterCallback<MouseMoveEvent>(OnMouseMoveEvent);

            _image = new Texture2D(Width, Height, TextureFormat.RGBA32, false);
            var half = Width / 2;
            var center = new float2(half, half);
            var color = Color.red;

            for (int y = 0; y < _image.height; y++)
            {
                for (int x = 0; x < _image.width; x++)
                {
                    // var color = x < half ? Color.blue : Color.cyan;
                    if (math.distancesq(center, new float2(x, y)) > (Width / 2) * (Width / 2))
                    {
                        _image.SetPixel(x, y, Color.gray);
                        continue;
                    }

                    var y1 = math.sin(math.PI / 3) * (x - half) + half;

                    var y2 = -math.cos(math.PI / 3) * (x - half) + half;

                    if (y > y1 && x < half)
                    {
                        color = Color.blue;
                    }
                    else if (y > y2 && x > half)
                    {
                        color = Color.red;
                    }
                    else
                    {
                        color = Color.green;
                    }


                _image.SetPixel(x, y, color);
                }
            }

            _image.Apply();
            _background.style.backgroundImage = _image;
        }

        public void SetColor(string guid, Color color)
        {
            if (_colors.TryGetValue(guid, out var col))
            {
                return;
            }

            _colors[guid] = color;
            // 分成多少份
            var count = _colors.Count;

            var perCount = Mathf.PI / count;

            // foreach (var (key, value) in _colors)
            // {
            //     for (int x = 0; x < _image.width; x++)
            //     {
            //         var center = new float2(Width / 2f, Width / 2f);
            //         for (int y = 0; y < _image.height; y++)
            //         {
            //             if (math.distancesq(center, new float2(x, y)) > Width * Width)
            //             {
            //                 continue;
            //             }
            //             _image.SetPixel(x, y, value);
            //         }
            //     }
            // }
            // _image.Apply();
            // _background.style.backgroundImage = _image;
        }

        private bool _mouseDown = false;

        private void OnMouseEnterEvent(MouseEnterEvent evt)
        {
        }

        private void OnMouseLevelEvent(MouseLeaveEvent evt)
        {
        }

        private void OnMouseMoveEvent(MouseMoveEvent evt)
        {
        }

        private void OnMouseDownEvent(MouseDownEvent evt)
        {
            _mouseDown = true;
        }

        private void OnMouseUpEvent(MouseUpEvent evt)
        {
            _mouseDown = false;
        }


        public void SetDesc(string data)
        {
            _des.text = data;
        }
    }
}