using System.Collections.Generic;
using UnityEngine;
using UnityEditor.Experimental.GraphView;

namespace LevelEditorTools.Editor.Nodes
{
    public class LevelNodeProvider : ScriptableObject, ISearchWindowProvider
    {
        
        /// <summary>
        /// delegate回调方法
        /// </summary>
        public event System.Func<SearchTreeEntry, SearchWindowContext, bool> OnSelectEntryHandler; 
        
        public List<SearchTreeEntry> CreateSearchTree(SearchWindowContext context)
        {
            List<SearchTreeEntry> entries = new List<SearchTreeEntry>();

            entries.Add(new SearchTreeGroupEntry(new GUIContent("Create Node")));
            entries.Add(new SearchTreeEntry(new GUIContent("Scene Node")) {level = 1, userData = typeof(SceneNode)});
            entries.Add(new SearchTreeEntry(new GUIContent("Ground Node")){level = 1, userData = typeof(GroundNode)});
            entries.Add(new SearchTreeEntry(new GUIContent("Door Node")) {level = 1, userData = typeof(DoorNode)});
            entries.Add(new SearchTreeEntry(new GUIContent("Wall Node")){level = 1, userData = typeof(WallNode)});
            entries.Add(new SearchTreeEntry(new GUIContent("Obstacle Node")){level = 1, userData = typeof(ObstacleNode)});
            entries.Add(new SearchTreeEntry(new GUIContent("GameObject Node")){level = 1, userData = typeof(GameObjectNode)});
            
            return entries;
        }

        public bool OnSelectEntry(SearchTreeEntry searchTreeEntry, SearchWindowContext context)
        {
            return OnSelectEntryHandler != null && OnSelectEntryHandler.Invoke(searchTreeEntry, context);
        }
    }
}