using System;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using LevelEditorTools.Nodes;
using UnityEngine.UIElements;

namespace LevelEditorTools.Editor.Nodes
{
    public class BaseNode : Node
    {
        public event Action<BaseNode, bool> onSelected;

        private string _title;

        private string _guidValue;

        protected BaseScriptable _state;

        public override void BuildContextualMenu(ContextualMenuPopulateEvent evt)
        {
            base.BuildContextualMenu(evt);
            evt.menu.AppendAction( "Editor Script", EditorScriptContextMenu);
        }

        protected virtual void  EditorScriptContextMenu(DropdownMenuAction obj)
        {
            
        }

        
        
        public BaseScriptable State
        {
            get => _state;
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

        public virtual bool DrawInspectorGUI()
        {
            bool hasChange = false;
            string title = EditorGUILayout.TextField("Title", _state.Title, GUILayout.ExpandHeight(true));
            if (_state.Title != title)
            {
                _state.Title = title;
                hasChange = true;
            }
            EditorGUILayout.TextField("Guid:",_state.Guid, GUILayout.ExpandWidth(true));
            return hasChange;
        }
    }
}