using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace MapEditor
{
    public class ResData
    {
        public string Key;
        public string ResPath;
        public Color Color = Color.black;
    }

    public class ResGroupData
    {
        public string Key;
        public List<ResData> Data = new List<ResData>();
    }

    public class MapHierarchyView : GraphView
    {
        public new class UxmlFactory : UxmlFactory<MapHierarchyView, UxmlTraits>
        {
        }

        private readonly List<ResGroupData> _groupList = new List<ResGroupData>();

        private readonly List<bool> _groupFoldoutList = new List<bool>();

        public MapHierarchyView()
        {
            // Inport USS
            var styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>(AssetDatabase.GUIDToAssetPath("3809fd0b7d9d4ab495256122da5f0407"));
            styleSheets.Add(styleSheet);
        }

        public void HierarchyContainerHandler()
        {
            var count = _groupList.Count;
            for (var i = 0; i < count; i++)
            {
                var groupData = _groupList[i];
                EditorGUILayout.BeginHorizontal();
                _groupFoldoutList[i] = EditorGUILayout.Foldout(_groupFoldoutList[i], groupData.Key);
                groupData.Key = EditorGUILayout.TextField(groupData.Key);
                if (GUILayout.Button("+", GUILayout.Width(20)))
                {
                    groupData.Data.Add(new ResData());
                }

                EditorGUILayout.EndHorizontal();
                if (!_groupFoldoutList[i]) continue;
                for (int j = 0; j < groupData.Data.Count; j++)
                {
                    var resData = groupData.Data[j];
                    EditorGUILayout.LabelField($"--:{resData.Key}");
                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.Toggle(false, GUILayout.Width(20), GUILayout.Height(20));
                    var go = AssetDatabase.LoadAssetAtPath<GameObject>(resData.ResPath);
                    var obj = EditorGUILayout.ObjectField("", go, typeof(GameObject), false, GUILayout.Width(150));
                    resData.ResPath = AssetDatabase.GetAssetPath(obj);
                    if (obj != null)
                    {
                        resData.Key = obj.name;
                    }

                    resData.Color = EditorGUILayout.ColorField(resData.Color, GUILayout.Width(50));

                    if (GUILayout.Button("-", GUILayout.Width(20)))
                    {
                        groupData.Data.RemoveAt(j);
                    }

                    EditorGUILayout.EndHorizontal();
                }
            }
        }

        public void BtnAddHierarchyGroup_OnClick()
        {
            // _groupList.Add(new ResGroupData());
            // _groupFoldoutList.Add(true);

            var foldout = new FoldoutList();
            this.Add(foldout);
        }
    }
}