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

        private int _baseValue = 500;

        public MapSceneView()
        {
            var styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>(AssetDatabase.GUIDToAssetPath("3809fd0b7d9d4ab495256122da5f0407"));
            this.styleSheets.Add(styleSheet);

            graphViewChanged = OnGraphViewChanged;
        }

        private GraphViewChange OnGraphViewChanged(GraphViewChange graphviewchange)
        {
            Debug.Log("Changed");
            return graphviewchange;
        }

        public void InitDrawParentElement(VisualElement element)
        {
            _parentElement = element;

            DrawGrid();
        }

        private void DrawGrid()
        {
            _parentElement.Clear();

            var columnCount = _rightValue - _leftValue + 1;
            var rowCount = _upValue - _downValue + 1;

            var size = this.viewport.localBound.size;
            var grid = _gridSize.x;
            var width = columnCount * grid;
            var height = rowCount * grid;
            var centerX = Mathf.RoundToInt(size.x / 2);
            var centerY = Mathf.RoundToInt(size.y / 2);

            // 右上角坐标
            var rightUp = new Vector2(centerX - width / 2, centerY - height / 2);

            for (var i = _downValue; i <= _upValue; i++)
            {
                var box = new Box
                {
                    style =
                    {
                        width = width,
                        height = _gridSize.y,
                        flexDirection = FlexDirection.Row
                    },
                };

                for (var j = _leftValue; j <= _rightValue; j++)
                {
                    var basPos = new Vector3((j - _leftValue) * grid, (i - _downValue) * grid, 0);
                    var pos = new Vector3(rightUp.x, rightUp.y, 0) + basPos;
                    var btn = new ButtonBox();
                    btn.SetColor("123456", Color.red);
                    box.Add(btn);
                }

                _parentElement.Add(box);
            }
        }

        private bool _isDraw = false;

        public void SceneContainerHandler()
        {
            if (_isDraw) return;
            _isDraw = true;
        }

        // 绘制正六边型
        private void DrawHexagon(Vector3 center, float size, int i, int j)
        {
            var baseAngle = Mathf.PI / 3;
            var angle = i % 2 == 0 ? baseAngle : 0;

            var angle_0 = new Vector3(center.x + size, center.y, 0);
            var angle_60 = new Vector3(center.x + size / 2, center.y + size * Mathf.Sin(Mathf.PI / 3));
            var angle_120 = new Vector3(center.x - size / 2, center.y + size * Mathf.Sin(Mathf.PI / 3));
            var angle_180 = new Vector3(center.x - size, center.y, 0);
            var angle_240 = new Vector3(center.x - size / 2, center.y - size * Mathf.Sin(Mathf.PI / 3));
            var angle_300 = new Vector3(center.x + size / 2, center.y - size * Mathf.Sin(Mathf.PI / 3));

            Handles.DrawLine(angle_0, angle_60);
            Handles.DrawLine(angle_60, angle_120);
            Handles.DrawLine(angle_120, angle_180);
            Handles.DrawLine(angle_180, angle_240);
            Handles.DrawLine(angle_240, angle_300);
            Handles.DrawLine(angle_300, angle_0);
        }

        private Vector3 GetAnglePosition(Vector3 center, float size, float angle)
        {
            var dir = Quaternion.AngleAxis(angle, Vector3.forward) * (Vector3.right - center) + center;
            var ray = new UnityEngine.Ray(center, dir.normalized);
            return ray.GetPoint(size);
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