using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Editor.Nodes
{
    [System.Serializable]
    public class GameObjectScriptable : ScriptableObject
    {
        public Vector3 Postion = Vector3.zero;
        public Quaternion Rotation = Quaternion.identity;
        public Vector3 Scale = Vector3.one;
        public GameObject Template;
    }
}
