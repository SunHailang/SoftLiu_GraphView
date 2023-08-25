using System.Collections.Generic;
using UnityEngine;

namespace MapEditor
{
    [System.Serializable]
    public class MapItemData
    {
        public int ItemGroupUnique;
        public string ItemGroup;
        public string ItemPath;
        public Color ItemColor;
    }
    
    [System.Serializable]
    public class MapResData : ScriptableObject
    {
        public List<MapItemData> MapItemList = new List<MapItemData>();
    }
}