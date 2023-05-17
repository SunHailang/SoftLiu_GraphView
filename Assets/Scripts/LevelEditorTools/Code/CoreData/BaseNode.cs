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

        
        
        public BaseScriptable State => _state;

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

        public void SetTitle(string stateTitle = "")
        {
            stateTitle = string.IsNullOrEmpty(stateTitle) ? nameof(BaseNode) : stateTitle;

            this.title = stateTitle;
            _state.Title = stateTitle;
        }

        public void SetGuid(string stateGuid = "")
        {
            _guidValue = string.IsNullOrEmpty(stateGuid) ? Guid.NewGuid().ToString() : stateGuid;
            _state.Guid = _guidValue;
        }

        public virtual bool DrawInspectorGUI()
        {
            var hasChange = false;
            var stateTitle = EditorGUILayout.TextField("Title", _state.Title, GUILayout.ExpandHeight(true));
            if (_state.Title != stateTitle)
            {
                _state.Title = stateTitle;
                SetTitle(stateTitle);
                hasChange = true;
            }
            EditorGUILayout.TextField("Guid:",_state.Guid, GUILayout.ExpandWidth(true));
            return hasChange;
        }
    }
}