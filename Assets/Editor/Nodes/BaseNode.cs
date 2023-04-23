using System;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace Editor.Nodes
{
    public class BaseNode : Node
    {
        public event Action<BaseNode, bool> onSelected;

        private string _title;
        public string Title => _title;

        private string _guidValue;
        public string GuidValue => _guidValue;

        public State state;

        public BaseNode()
        {
            _guidValue = Guid.NewGuid().ToString();
            state = ScriptableObject.CreateInstance<State>();
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
        }
    }
}