using System.Collections.Generic;
using System.Linq;
using Editor.GraphViews;
using UnityEditor.Experimental.GraphView;

/**
 * @Author 
 * @FileName GraphSceneSaveUtility.cs
 * @Data 2023年4月23日
**/
public class GraphSceneSaveUtility
{
    private static GraphSceneSaveUtility _instance = null;
    private readonly static object _lock = new object();
    private SceneGraphView _sceneGraphView;

    private List<Edge> edges => _sceneGraphView.edges.ToList();
    private List<State> nodes => _sceneGraphView.nodes.ToList().Cast<State>().ToList();


    public static GraphSceneSaveUtility GetInstance(SceneGraphView sceneGraphView)
    {
        lock (_lock)
        {
            if (_instance == null)
            {
                _instance = new GraphSceneSaveUtility()
                {
                    _sceneGraphView = sceneGraphView
                };
            }

            return _instance;
        }
    }


    public void Save()
    {
        
    }

    public void Load()
    {
        
    }
    
}