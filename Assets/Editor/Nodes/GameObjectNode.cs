using System.Collections;
using Unity.EditorCoroutines.Editor;
using UnityEngine;
using UnityEditor.Experimental.GraphView;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace GraphEditor.Nodes
{
    public class GameObjectNode : BaseNode
    {
        private GameObjectScriptable _scriptable;

        public GameObjectNode()
        {
            // 添加一个 input
            Port input = GraphViewUtils.GetInstantiatePort(this, Orientation.Vertical, Direction.Input, Port.Capacity.Single, typeof(int));
            this.inputContainer.Add(input);

            _scriptable = new GameObjectScriptable();
            State = _scriptable;
        }

        public void RefreshTempleGo(EditorWindow window)
        {
            GameObject valueGo = AssetDatabase.LoadAssetAtPath<GameObject>(AssetDatabase.GUIDToAssetPath(_scriptable.TemplateGo));
            var objField = new ObjectField()
            {
                objectType = typeof(GameObject),
                allowSceneObjects = false,
                value = valueGo
            };

            Image image = new Image();
            window.StartCoroutine(LoadPreview(valueGo, image));
            objField.RegisterValueChangedCallback(v =>
            {
                GameObject go = v.newValue as GameObject;
                window.StartCoroutine(LoadPreview(go, image));
            });
            this.Add(objField);
            this.Add(image);
        }

        private IEnumerator LoadPreview(GameObject go, Image image)
        {
            if (go != null && image != null)
            {
                _scriptable.TemplateGo = AssetDatabase.AssetPathToGUID(AssetDatabase.GetAssetPath(go));
                Texture2D preview = AssetPreview.GetAssetPreview(go);
                while (preview == null)
                {
                    preview = AssetPreview.GetAssetPreview(go);
                    yield return new WaitForSeconds(0.5f);
                }

                image.image = preview != null ? preview : AssetPreview.GetMiniThumbnail(go);
            }
        }


        public override void DrawInspectorGUI()
        {
            base.DrawInspectorGUI();
            EditorGUILayout.LabelField("Offset", GUILayout.ExpandWidth(true));
            _scriptable.Position = EditorGUILayout.Vector3Field("Position", _scriptable.Position, GUILayout.ExpandWidth(true));
            _scriptable.Rotation = EditorGUILayout.Vector3Field("Rotation", _scriptable.Rotation, GUILayout.ExpandWidth(true));
            _scriptable.Scale = EditorGUILayout.Vector3Field("Scale", _scriptable.Scale, GUILayout.ExpandWidth(true));
            GUILayout.Box("", GUILayout.ExpandWidth(true), GUILayout.Height(1));
            EditorGUILayout.LabelField("Attachment", GUILayout.ExpandWidth(true));
            _scriptable.Probability = EditorGUILayout.Slider("Probability", _scriptable.Probability, 0, 1, GUILayout.ExpandWidth(true));
            _scriptable.ForceStatic = EditorGUILayout.Toggle("ForceStatic", _scriptable.ForceStatic);
        }
    }
}