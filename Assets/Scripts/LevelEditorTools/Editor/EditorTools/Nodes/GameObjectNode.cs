using System.Collections;
using Unity.EditorCoroutines.Editor;
using UnityEngine;
using UnityEditor.Experimental.GraphView;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;
using LevelEditorTools.Nodes;

namespace LevelEditorTools.Editor.Nodes
{
    public class GameObjectNode : BaseNode
    {
        public GameObjectNode()
        {
            // 添加一个 input
            Port input = GraphViewUtils.GetInstantiatePort(this, Orientation.Vertical, Direction.Input, Port.Capacity.Single, typeof(GameObjectNode));
            this.inputContainer.Add(input);

            _state = new GameObjectScriptable();
        }
        
        protected override void EditorScriptContextMenu(DropdownMenuAction obj)
        {
            GraphViewUtils.OpenCodeEditor("GameObjectNode");
        }

        public void RefreshTempleGo(EditorWindow window)
        {
            if (_state is GameObjectScriptable scriptable)
            {
                GameObject valueGo = AssetDatabase.LoadAssetAtPath<GameObject>(AssetDatabase.GUIDToAssetPath(scriptable.TemplateGo));
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
        }

        private IEnumerator LoadPreview(GameObject go, Image image)
        {
            if (_state is GameObjectScriptable scriptable && go != null && image != null)
            {
                scriptable.TemplateGo = AssetDatabase.AssetPathToGUID(AssetDatabase.GetAssetPath(go));
                Texture2D preview = AssetPreview.GetAssetPreview(go);
                while (preview == null)
                {
                    preview = AssetPreview.GetAssetPreview(go);
                    yield return new WaitForSeconds(0.5f);
                }

                image.image = preview != null ? preview : AssetPreview.GetMiniThumbnail(go);
            }
        }


        public override bool DrawInspectorGUI()
        {
            bool hasChange = base.DrawInspectorGUI();
            if (_state is GameObjectScriptable scriptable)
            {
                EditorGUILayout.LabelField("Offset", GUILayout.ExpandWidth(true));
                Vector3 pos = EditorGUILayout.Vector3Field("Position", scriptable.Position, GUILayout.ExpandWidth(true));
                if (scriptable.Position != pos)
                {
                    scriptable.Position = pos;
                    hasChange = true;
                }

                Vector3 rot = EditorGUILayout.Vector3Field("Rotation", scriptable.Rotation, GUILayout.ExpandWidth(true));
                if (scriptable.Rotation != rot)
                {
                    scriptable.Rotation = rot;
                    hasChange = true;
                }

                Vector3 scale = EditorGUILayout.Vector3Field("Scale", scriptable.Scale, GUILayout.ExpandWidth(true));
                if (scriptable.Scale != scale)
                {
                    scriptable.Scale = scale;
                    hasChange = true;
                }

                GUILayout.Box("", GUILayout.ExpandWidth(true), GUILayout.Height(1));
                EditorGUILayout.LabelField("Attachment", GUILayout.ExpandWidth(true));
                float prob = EditorGUILayout.Slider("Probability", scriptable.Probability, 0, 1, GUILayout.ExpandWidth(true));
                if (scriptable.Probability != prob)
                {
                    scriptable.Probability = prob;
                    hasChange = true;
                }

                bool isStatic = EditorGUILayout.Toggle("ForceStatic", scriptable.ForceStatic);
                if (scriptable.ForceStatic != isStatic)
                {
                    scriptable.ForceStatic = isStatic;
                    hasChange = true;
                }
            }

            return hasChange;
        }
    }
}