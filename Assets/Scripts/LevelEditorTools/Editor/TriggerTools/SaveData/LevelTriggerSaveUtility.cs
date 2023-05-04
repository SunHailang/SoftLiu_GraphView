using System.Collections.Generic;
using System.Linq;
using LevelEditorTools.Editor.Nodes;
using LevelEditorTools.GraphViews;
using UnityEditor.Experimental.GraphView;

namespace LevelEditorTools.Save
{
    public class LevelTriggerSaveUtility
    {
        private static LevelTriggerSaveUtility _instance = null;
        private static readonly object _lock = new object();
        private static SceneTriggerView _sceneGraphView;

        private List<Edge> edges => _sceneGraphView.edges.ToList();
        private List<BaseNode> nodes => _sceneGraphView.nodes.ToList().Cast<BaseNode>().ToList();
        
        
        public static LevelTriggerSaveUtility GetInstance(SceneTriggerView sceneGraphView)
        {
            lock (_lock)
            {
                _instance ??= new LevelTriggerSaveUtility();
                _sceneGraphView = sceneGraphView;
                return _instance;
            }
        }
        
        
        public void Load(LevelTriggerContainer container)
        {
            
        }

        public string Save(string path)
        {
            return path;
        }
    }
}