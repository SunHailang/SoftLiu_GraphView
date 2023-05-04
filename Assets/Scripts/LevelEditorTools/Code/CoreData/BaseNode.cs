using System;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using LevelEditorTools.Nodes;

namespace LevelEditorTools.Editor.Nodes
{
    public class BaseNode : Node
    {
        public event Action<BaseNode, bool> onSelected;

        private string _title;

        private string _guidValue;

        private BaseScriptable _state;

        public BaseScriptable State
        {
            get => _state;
            protected set
            {
                _state = value;
                _state.Guid = _guidValue;
            }
        }

        public override void OnSelected()
        {
            base.OnSelected();
            onSelected?.Invoke(this, true);
        }

        public override void OnUnselected()
        {
            onSelected?.Invoke(this, false);
            base.OnUnselected();
        }

        public void SetTitle(string title = "")
        {
            if (string.IsNullOrEmpty(title))
            {
                _title = nameof(BaseNode);
            }
            else
            {
                _title = title;
            }

            this.title = _title;
            _state.Title = _title;
        }

        public void SetGuid(string _guid = "")
        {
            if (string.IsNullOrEmpty(_guid))
            {
                _guidValue = Guid.NewGuid().ToString();
            }
            else
            {
                _guidValue = _guid;
            }
            _state.Guid = _guidValue;
        }

        public virtual void DrawInspectorGUI()
        {
            EditorGUILayout.TextField("Title", _state.Title, GUILayout.ExpandHeight(true));
            EditorGUILayout.TextField("Guid:",_state.Guid, GUILayout.ExpandWidth(true));
        }
    }
}