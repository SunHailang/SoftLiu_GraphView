using UnityEngine;

namespace LevelEditorTools.Nodes
{
    
    public enum TriggerStateEnum
    {
        None = 0,
        Enter = 1 << 0,
        Stay = 1<<1,
        Exist = 1 << 2,
        EntryAndExist = Enter | Exist,
    }
    
    [System.Serializable]
    public class BoxTriggerScriptable : BaseScriptable
    {
        public Vector3 Position = Vector3.zero;
        public Vector3 Scale = Vector3.one;

        public TriggerStateEnum TriggerState = TriggerStateEnum.None;
    }
}