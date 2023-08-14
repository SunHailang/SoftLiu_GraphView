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

        private HashSet<string> _guidList = new HashSet<string>();
        private List<Color> _colorList = new List<Color>();

        public ButtonBox()
        {
            // Import UXML // "Assets/Scripts/MapEditorCode/Editor/VisualElements/ButtonBox.uxml"
            var visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(AssetDatabase.GUIDToAssetPath("b2c86cd3e11c0cd4c805550134ac4a6b"));
            visualTree.CloneTree(this);
            _background = this.Q<VisualElement>("BtnElement");
            _des = this.Q<Label>("Desc");

            this.style.width = Width;
            this.style.height = Height;

            RegisterCallback<MouseUpEvent>(OnMouseUpEvent);

            _image = new Texture2D(Width, Height, TextureFormat.RGBA32, false);
            _background.style.backgroundImage = _image;
        }

        public void SetColor(string guid, Color color)
        {
            if (!_guidList.Add(guid))
            {
                return;
            }

            _colorList.Add(color);
            
            SetColor(_colorList);
        }

        private void SetColor(List<Color> colors)
        {
            var len = colors.Count;
            var half = Width / 2;
            var center = new float2(half, half);
            switch (len)
            {
                case 1:
                {
                    for (var x = 0; x < _image.width; x++)
                    {
                        for (var y = 0; y < _image.height; y++)
                        {
                            if (math.distancesq(center, new float2(x, y)) > Width * Width)
                            {
                                continue;
                            }
                            _image.SetPixel(x, y, colors[0]);
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
                            if (math.distancesq(center, new float2(x, y)) > Width * Width)
                            {
                                continue;
                            }
                            var color = x < half ? colors[0] : colors[1];
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
                                _image.SetPixel(x, y, Color.gray);
                                continue;
                            }

                            var y1 = math.sin(math.PI / 3) * (x - half) + half;

                            var y2 = -math.cos(math.PI / 3) * (x - half) + half;
                            Color color;
                            if (y > y1 && x < half)
                            {
                                color =colors[0];
                            }
                            else if (y > y2 && x > half)
                            {
                                color =colors[1];
                            }
                            else
                            {
                                color = colors[2];
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
                                _image.SetPixel(x, y, Color.gray);
                                continue;
                            }

                            var y1 = math.sin(math.PI / 4) * (x - half) + half;

                            var y2 = -math.cos(math.PI / 3) * (x - half) + half;
                            Color color;
                            if (y >= 0 && y < half && x >= 0 && x < half)
                            {
                                color =colors[0];
                            }
                            else if (y >= half && x >= 0 && x < half)
                            {
                                color =colors[1];
                            }
                            else if(y >= 0 && y < half && x >= half)
                            {
                                color = colors[2];
                            }
                            else
                            {
                                color = colors[3];
                            }
                            _image.SetPixel(x, y, color);
                        }
                    }
                    _image.Apply();
                    break;
                }
            }
        }


        private void OnMouseUpEvent(MouseUpEvent evt)
        {
            if (evt.button == 0)
            {
                // 左键
            }
            else if (evt.button == 1)
            {
                // 右键
            }
        }


        public void SetDesc(string data)
        {
            _des.text = data;
        }
    }
}