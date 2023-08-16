using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace MapEditor
{
    public class MapSceneView : GraphView
    {
        public new class UxmlFactory : UxmlFactory<MapSceneView, UxmlTraits>
        {
        }

        private VisualElement _parentElement;

        private Vector2 _gridSize = new Vector2(50, 50);

        private int _leftValue = 500;
        private int _upValue = 500;
        private int _rightValue = 500;
        private int _downValue = 500;

        public MapSceneView()
        {
            Insert(0, new GridBackground());
            // 滚轮
            this.SetupZoom(ContentZoomer.DefaultMinScale, ContentZoomer.DefaultMaxScale);
            this.AddManipulator(new ContentZoomer());
            // 可以选中单个Node
            this.AddManipulator(new SelectionDragger());
            // 按住 Alt键 可以拖动
            this.AddManipulator(new ContentDragger());
            // 可以框选多个Node
            this.AddManipulator(new RectangleSelector());
            
            var styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>(AssetDatabase.GUIDToAssetPath("3809fd0b7d9d4ab495256122da5f0407"));
            this.styleSheets.Add(styleSheet);
        }

        public void InitDrawParentElement(VisualElement element)
        {
            _parentElement = element;
            DrawGrid();
        }

        private void DrawGrid()
        {
            _parentElement.Clear();

            for (var i = _downValue; i <= _upValue; i++)
            {
                var box = new Box
                {
                    style =
                    {
                        width = (_rightValue - _leftValue + 1) * _gridSize.x,
                        height = _gridSize.y,
                        flexDirection = FlexDirection.Row
                    }
                };

                for (var j = _leftValue; j <= _rightValue; j++)
                {
                    var btn = new ButtonBox();
                    btn.SetSelectCallback(SelectCallback);
                    btn.SetIndex(i, j, "");
                    btn.SetColor("", new Color(0, 0, 0, 0));
                    btn.SetSelect(_curSelectButtonBox != null && _curSelectButtonBox.EqualIndex(btn.IndexX, btn.IndexY));
                    box.Add(btn);
                }

                _parentElement.Add(box);
            }
        }

        private ButtonBox _curSelectButtonBox = null;

        private void SelectCallback(ButtonBox data)
        {
            if (_curSelectButtonBox != null && !_curSelectButtonBox.EqualIndex(data.IndexX, data.IndexY))
            {
                _curSelectButtonBox.SetSelect(false);
            }

            _curSelectButtonBox = data;
            // TODO
        }

        private bool _isDraw = false;

        public void SceneContainerHandler()
        {
            if (_isDraw) return;
            _isDraw = true;
        }
        

        public void BtnLeftLine_OnClick(bool isAdd = true)
        {
            _leftValue = isAdd ? _leftValue - 1 : _leftValue + 1;
            if (_leftValue >= _rightValue) _leftValue = _rightValue;
            DrawGrid();
        }


        public void BtnTopLine_OnClick(bool isAdd = true)
        {
            _upValue = isAdd ? _upValue + 1 : _upValue - 1;
            if (_upValue <= _downValue) _upValue = _downValue;
            DrawGrid();
        }

        public void BtnRightLine_OnClick(bool isAdd = true)
        {
            _rightValue = isAdd ? _rightValue + 1 : _rightValue - 1;
            if (_rightValue <= _leftValue) _rightValue = _leftValue;
            DrawGrid();
        }

        public void BtnBottomLine_OnClick(bool isAdd = true)
        {
            _downValue = isAdd ? _downValue - 1 : _downValue + 1;
            if (_downValue >= _upValue) _downValue = _upValue;
            DrawGrid();
        }
    }
}