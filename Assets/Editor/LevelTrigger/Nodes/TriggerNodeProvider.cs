using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace GraphEditor.LevelTrigger
{
    public class TriggerNodeProvider : ScriptableObject, ISearchWindowProvider
    {
        /// <summary>
        /// delegate回调方法
        /// </summary>
        public event System.Func<SearchTreeEntry, SearchWindowContext, bool> OnSelectEntryHandler; 
        
        
        public List<SearchTreeEntry> CreateSearchTree(SearchWindowContext context)
        {
            var entries = new List<SearchTreeEntry>();
            // 创建一个一级菜单
            entries.Add(new SearchTreeGroupEntry(new GUIContent("Create Node")));
            // 创建一个二级菜单
            //entries.Add(new SearchTreeGroupEntry(new GUIContent("Example")) {level = 1});
            entries.Add(new SearchTreeEntry(new GUIContent("BoxTrigger Node")) {level = 1, userData = typeof(BoxTriggerNode)});
            entries.Add(new SearchTreeEntry(new GUIContent("ConditionTrigger Node")) {level = 1, userData = typeof(ConditionTriggerNode)});
            return entries;
        }

        

        public bool OnSelectEntry(SearchTreeEntry searchTreeEntry, SearchWindowContext context)
        {
            return OnSelectEntryHandler != null && OnSelectEntryHandler(searchTreeEntry, context);
        }
    }
}