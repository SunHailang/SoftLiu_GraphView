using System;
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
    public enum MouseKeyType
    {
        None = -1,
        Left = 0,
        Right = 1,
        Middle = 2
    }

    public class BoxColorData
    {
        public string DataPath;
        public Color DataColor;

        public static BoxColorData Zero = new BoxColorData
        {
            DataPath = "",
            DataColor = Color.clear
        };
    }

    public class ButtonBox : VisualElement
    {
        public class Factory : UxmlFactory<ButtonBox, UxmlTraits>
        {
        }

        private VisualElement _background;
        private Label _des;
        private Image _selectImage;
        private Image _entryImage;

        private Texture2D _image;

        private const int Width = 50;
        private const int Height = 50;

        public readonly List<BoxColorData> BoxColorList = new List<BoxColorData>();

        private System.Action<ButtonBox, MouseKeyType> _selectCallback;

        private int _indexX = int.MinValue;
        public int IndexX => _indexX;
        private int _indexY = int.MinValue;
        public int IndexY => _indexY;

        public ButtonBox()
        {
            // Import UXML // "Assets/Scripts/MapEditorCode/Editor/VisualElements/ButtonBox.uxml"
            var visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(AssetDatabase.GUIDToAssetPath("b2c86cd3e11c0cd4c805550134ac4a6b"));
            visualTree.CloneTree(this);
            _background = this.Q<VisualElement>("BtnElement");
            _des = this.Q<Label>("Desc");
            _entryImage = this.Q<Image>("EntryImage");
            _selectImage = this.Q<Image>("SelectImage");

            this.style.width = Width;
            this.style.height = Height;

            RegisterCallback<MouseUpEvent>(OnMouseUpEvent);
            RegisterCallback<MouseDownEvent>(OnMouseDownEvent);
            RegisterCallback<MouseEnterEvent>(OnMouseEntryEvent);
            RegisterCallback<MouseLeaveEvent>(OnMouseLeaveEvent);

            _image = new Texture2D(Width, Height, TextureFormat.RGBA32, false);
            _background.style.backgroundImage = _image;
        }

        public void SetSelectCallback(System.Action<ButtonBox, MouseKeyType> callback)
        {
            _selectCallback = callback;
        }

        public void SetIndex(int x, int y)
        {
            _indexX = x;
            _indexY = y;
        }

        public bool EqualIndex(int x, int y)
        {
            return x == _indexX && y == _indexY;
        }

        public void RemoveColor(string path, Color color)
        {
            for (int i = BoxColorList.Count - 1; i >= 0; i--)
            {
                var data = BoxColorList[i];
                if (data.DataPath == path)
                {
                    BoxColorList.RemoveAt(i);
                    break;
                }
            }

            SetColor(BoxColorList);
        }

        public void SetColor(string path, Color color)
        {
            if (!string.IsNullOrEmpty(path))
            {
                BoxColorList.Add(new BoxColorData
                {
                    DataPath = path,
                    DataColor = color
                });
            }

            SetColor(BoxColorList);
        }

        private void SetColor(List<BoxColorData> colors)
        {
            var len = colors.Count;
            var half = Width / 2;
            var center = new float2(half, half);
            switch (len)
            {
                case 0:
                {
                    for (var x = 0; x < _image.width; x++)
                    {
                        for (var y = 0; y < _image.height; y++)
                        {
                            _image.SetPixel(x, y, Color.clear);
                        }
                    }
                    _image.Apply();
                }
                    break;
                case 1:
                {
                    for (var x = 0; x < _image.width; x++)
                    {
                        for (var y = 0; y < _image.height; y++)
                        {
                            if (math.distancesq(center, new float2(x, y)) > half * half)
                            {
                                _image.SetPixel(x, y, Color.clear);
                                continue;
                            }

                            _image.SetPixel(x, y, colors[0].DataColor);
                        }
                    }

                    _image.Apply();
                    break;
                }
                case 2:
                {
                    for (var x = 0; x < _image.width; x++)
                    {
                        for (var y = 0; y < _image.height; y++)
                        {
                            if (math.distancesq(center, new float2(x, y)) > half * half)
                            {
                                _image.SetPixel(x, y, Color.clear);
                                continue;
                            }

                            var color = x < half ? colors[0].DataColor : colors[1].DataColor;
                            _image.SetPixel(x, y, color);
                        }
                    }

                    _image.Apply();
                    break;
                }
                case 3:
                {
                    for (int y = 0; y < _image.height; y++)
                    {
                        for (int x = 0; x < _image.width; x++)
                        {
                            if (math.distancesq(center, new float2(x, y)) > (Width / 2) * (Width / 2))
                            {
                                _image.SetPixel(x, y, Color.clear);
                                continue;
                            }

                            var y1 = math.sin(math.PI / 3) * (x - half) + half;

                            var y2 = -math.cos(math.PI / 3) * (x - half) + half;
                            Color color;
                            if (y > y1 && x < half)
                            {
                                color = colors[0].DataColor;
                            }
                            else if (y > y2 && x > half)
                            {
                                color = colors[1].DataColor;
                            }
                            else
                            {
                                color = colors[2].DataColor;
                            }

                            _image.SetPixel(x, y, color);
                        }
                    }

                    _image.Apply();
                    break;
                }
                case 4:
                {
                    for (int y = 0; y < _image.height; y++)
                    {
                        for (int x = 0; x < _image.width; x++)
                        {
                            if (math.distancesq(center, new float2(x, y)) > half * half)
                            {
                                _image.SetPixel(x, y, Color.clear);
                                continue;
                            }

                            var y1 = math.sin(math.PI / 4) * (x - half) + half;

                            var y2 = -math.cos(math.PI / 3) * (x - half) + half;
                            Color color;
                            if (y >= 0 && y < half && x >= 0 && x < half)
                            {
                                color = colors[0].DataColor;
                            }
                            else if (y >= half && x >= 0 && x < half)
                            {
                                color = colors[1].DataColor;
                            }
                            else if (y >= 0 && y < half && x >= half)
                            {
                                color = colors[2].DataColor;
                            }
                            else
                            {
                                color = colors[3].DataColor;
                            }

                            _image.SetPixel(x, y, color);
                        }
                    }

                    _image.Apply();
                    break;
                }
            }
        }

        private void OnMouseDownEvent(MouseDownEvent evt)
        {
            var keyType = MouseKeyType.Left;
            if (evt.button == 0)
            {
                // 左键
            }
            else if (evt.button == 1)
            {
                // 右键
                keyType = MouseKeyType.Right;
            }
            else if (evt.button == 2)
            {
                // 中键 只作为选取
                keyType = MouseKeyType.Middle;
            }

            SetSelect(true, keyType);
        }

        private void OnMouseUpEvent(MouseUpEvent evt)
        {
        }

        public void SetSelect(bool active, MouseKeyType keyType)
        {
            _selectImage.visible = active;
            if (active)
            {
                _selectCallback?.Invoke(this, keyType);
            }
        }

        private void OnMouseEntryEvent(MouseEnterEvent evt)
        {
            _entryImage.visible = true;
        }

        private void OnMouseLeaveEvent(MouseLeaveEvent evt)
        {
            _entryImage.visible = false;
        }


        public void SetDesc(string data)
        {
            _des.text = data;
        }
    }
}