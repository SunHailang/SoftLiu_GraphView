using System.Collections;
using System.Collections.Generic;
using LevelEditorTools.Nodes;
using UnityEngine;

namespace LevelEditorTools.Action
{
    public abstract class BaseAction
    {
        protected readonly int EventID;
        protected readonly TriggerStateEnum StateEnum;

        public BaseAction(int _event, TriggerStateEnum _state)
        {
            this.EventID = _event;
            this.StateEnum = _state;
        }
        public abstract void Execute();
    }
}