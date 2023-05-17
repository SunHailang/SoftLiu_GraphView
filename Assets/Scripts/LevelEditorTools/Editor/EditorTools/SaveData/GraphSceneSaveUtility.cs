using System.Collections.Generic;
using System.IO;
using System.Linq;
using LevelEditorTools.GraphViews;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using LevelEditorTools.Editor.Nodes;
using LevelEditorTools.Nodes;
using UnityEngine.UIElements;


namespace LevelEditorTools.Save
{
    public class GraphSceneSaveUtility
    {
        private static GraphSceneSaveUtility _instance = null;
        private static readonly object _lock = new object();
        private static SceneGraphView _sceneGraphView;

        private List<Edge> edges => _sceneGraphView.edges.ToList();
        private List<BaseNode> nodes => _sceneGraphView.nodes.ToList().Cast<BaseNode>().ToList();


        public static GraphSceneSaveUtility GetInstance(SceneGraphView sceneGraphView)
        {
            lock (_lock)
            {
                _instance ??= new GraphSceneSaveUtility();
                _sceneGraphView = sceneGraphView;
                return _instance;
            }
        }


        public string Save(string path)
        {
            //if (!edges.Any()) return path;
            SceneContainer container = AssetDatabase.LoadAssetAtPath<SceneContainer>(path);
            bool newFile = container == null;
            container ??= ScriptableObject.CreateInstance<SceneContainer>();

            Edge[] hasInputEdges = edges.Where(x => x.input != null).ToArray();
            container.NodeLinkDatas.Clear();
            foreach (Edge edge in hasInputEdges)
            {
                SceneNodeLinkData linkData = new SceneNodeLinkData();
                if (edge.output.node is BaseNode outputNode)
                {
                    linkData.OutputNodeGuid = outputNode.State.Guid;
                }

                if (edge.input.node is BaseNode inputNode)
                {
                    linkData.InputNodeGuid = inputNode.State.Guid;
                }

                container.NodeLinkDatas.Add(linkData);
            }

            container.NodeDatas.Clear();
            container.NodeSceneDatas.Clear();
            container.NodeBezierDatas.Clear();
            container.NodeGroundDatas.Clear();
            container.NodeDoorDatas.Clear();
            container.NodeObstacleDatas.Clear();
            container.NodeWallDatas.Clear();
            container.NodeGameObjectDatas.Clear();
            foreach (BaseNode node in nodes)
            {
                SceneNodeData nodeData = new SceneNodeData()
                {
                    NodeGuid = node.State.Guid,
                    NodePosition = node.GetPosition().position
                };
                container.NodeDatas.Add(nodeData);

                if (node.State is SceneScriptable sceneState)
                {
                    container.NodeSceneDatas.Add(sceneState);
                }
                else if (node.State is SceneBezierScriptable bezierState)
                {
                    container.NodeBezierDatas.Add(bezierState);
                }
                else if (node.State is GroundScriptable groundState)
                {
                    container.NodeGroundDatas.Add(groundState);
                }
                else if (node.State is DoorScriptable doorState)
                {
                    container.NodeDoorDatas.Add(doorState);
                }
                else if (node.State is ObstacleScriptable obstacle)
                {
                    container.NodeObstacleDatas.Add(obstacle);
                }
                else if (node.State is WallScriptable wall)
                {
                    container.NodeWallDatas.Add(wall);
                }
                else if (node.State is GameObjectScriptable goState)
                {
                    container.NodeGameObjectDatas.Add(goState);
                }
            }

            if (newFile)
            {
                string filePath = EditorUtility.SaveFilePanel("选择文件", Application.dataPath, "SceneContainer", "asset");
                if (!string.IsNullOrEmpty(filePath))
                {
                    path = filePath.Substring(Application.dataPath.Length - 6);
                    AssetDatabase.CreateAsset(container, path);
                }
            }
            else
            {
                EditorUtility.SetDirty(container);
            }
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            return path;
        }

        public void Load(SceneContainer container)
        {
            if (_sceneGraphView == null) return;
            List<BaseNode> list = new List<BaseNode>();
            // 1. 遍历生成 Node 节点
            foreach (SceneScriptable sceneData in container.NodeSceneDatas)
            {
                Vector2 pos = GraphViewUtils.GetNodePosition(sceneData.Guid, container.NodeDatas);
                BaseNode node = _sceneGraphView.CreateNode(typeof(SceneNode), pos, sceneData.Title, sceneData.Guid);
                if (node is SceneNode sceneNode)
                {
                    if (node.State is SceneScriptable data)
                    {
                        data.IsActive = sceneData.IsActive;
                        data.SceneType = sceneData.SceneType;
                        data.Radius = sceneData.Radius;
                        data.SceneScale = sceneData.SceneScale;
                        data.ScenePosition = sceneData.ScenePosition;
                    }

                    list.Add(sceneNode);
                }
            }
            
            foreach (SceneBezierScriptable bezierData in container.NodeBezierDatas)
            {
                Vector2 pos = GraphViewUtils.GetNodePosition(bezierData.Guid, container.NodeDatas);
                BaseNode node = _sceneGraphView.CreateNode(typeof(SceneBezierNode), pos, bezierData.Title, bezierData.Guid);
                if (node is SceneBezierNode bezierNode)
                {
                    if (node.State is SceneBezierScriptable data)
                    {
                        data.IsActive = bezierData.IsActive;
                        data.StartPosition = bezierData.StartPosition;
                        data.EndPosition = bezierData.EndPosition;
                        data.Width = bezierData.Width;
                        data.SegmentNumber = bezierData.SegmentNumber;
                        data.ControlPositionList = bezierData.ControlPositionList;
                    }

                    list.Add(bezierNode);
                }
            }
            
            foreach (GroundScriptable groundData in container.NodeGroundDatas)
            {
                Vector2 pos = GraphViewUtils.GetNodePosition(groundData.Guid, container.NodeDatas);
                BaseNode node = _sceneGraphView.CreateNode(typeof(GroundNode), pos, groundData.Title, groundData.Guid);
                if (node is GroundNode groundNode)
                {
                    if (groundNode.State is GroundScriptable groundScriptable)
                    {
                        groundScriptable.Seed = groundData.Seed;
                        groundScriptable.GroundSize = groundData.GroundSize;
                    }

                    list.Add(groundNode);
                }
            }

            foreach (DoorScriptable door in container.NodeDoorDatas)
            {
                Vector2 pos = GraphViewUtils.GetNodePosition(door.Guid, container.NodeDatas);
                BaseNode node = _sceneGraphView.CreateNode(typeof(DoorNode), pos, door.Title, door.Guid);
                if (node is DoorNode doorNode)
                {
                    if (doorNode.State is DoorScriptable doorScriptable)
                    {
                        doorScriptable.DoorSize = door.DoorSize;
                    }

                    list.Add(doorNode);
                }
            }

            foreach (ObstacleScriptable obstacle in container.NodeObstacleDatas)
            {
                Vector2 pos = GraphViewUtils.GetNodePosition(obstacle.Guid, container.NodeDatas);
                BaseNode node = _sceneGraphView.CreateNode(typeof(ObstacleNode), pos, obstacle.Title, obstacle.Guid);
                if (node is ObstacleNode obstacleNode)
                {
                    if (obstacleNode.State is ObstacleScriptable obstacleScriptable)
                    {
                        obstacleScriptable.Seed = obstacle.Seed;
                    }

                    list.Add(obstacleNode);
                }
            }

            foreach (WallScriptable wall in container.NodeWallDatas)
            {
                Vector2 pos = GraphViewUtils.GetNodePosition(wall.Guid, container.NodeDatas);
                BaseNode node = _sceneGraphView.CreateNode(typeof(WallNode), pos, wall.Title, wall.Guid);
                if (node is WallNode wallNode)
                {
                    if (wallNode.State is WallScriptable wallScriptable)
                    {
                        wallScriptable.Seed = wall.Seed;
                        wallScriptable.WallSize = wall.WallSize;
                    }

                    list.Add(wallNode);
                }
            }

            foreach (GameObjectScriptable objectData in container.NodeGameObjectDatas)
            {
                Vector2 pos = GraphViewUtils.GetNodePosition(objectData.Guid, container.NodeDatas);
                BaseNode node = _sceneGraphView.CreateNode(typeof(GameObjectNode), pos, objectData.Title, objectData.Guid);
                if (node is GameObjectNode goNode)
                {
                    if (goNode.State is GameObjectScriptable data)
                    {
                        data.Position = objectData.Position;
                        data.Rotation = objectData.Rotation;
                        data.Scale = objectData.Scale;
                        data.TemplateGo = objectData.TemplateGo;
                        data.Probability = objectData.Probability;
                        data.ForceStatic = objectData.ForceStatic;
                    }

                    goNode.RefreshTempleGo(_sceneGraphView.window);
                    list.Add(goNode);
                }
            }

            // 2. 连线
            foreach (SceneNodeLinkData data in container.NodeLinkDatas)
            {
                bool ret = GraphViewUtils.GetEdgeNode(data, list, out BaseNode outputNode, out BaseNode inputNode);
                if (ret)
                {
                    Port inputPort = inputNode.inputContainer.Query<Port>().ToList().FirstOrDefault();
                    Port outputPort = outputNode.outputContainer.Query<Port>().ToList().FirstOrDefault();
                    if (inputPort == null || outputPort == null) continue;
                    GraphViewUtils.AddEdgeByPorts(_sceneGraphView, outputPort, inputPort);
                }
            }
        }
    }
}