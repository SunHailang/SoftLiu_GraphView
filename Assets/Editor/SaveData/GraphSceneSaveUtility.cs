using System.Collections.Generic;
using System.IO;
using System.Linq;
using GraphEditor.GraphViews;
using GraphEditor.Nodes;
using NUnit.Framework;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;


namespace GraphEditor
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


        public void Save()
        {
            if (!edges.Any()) return;

            SceneContainer container = ScriptableObject.CreateInstance<SceneContainer>();

            Edge[] hasInputEdges = edges.Where(x => x.input != null).ToArray();

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
                else if (node.State is GroundScriptable groundState)
                {
                    container.NodeGroundDatas.Add(groundState);
                }
                else if (node.State is GameObjectScriptable goState)
                {
                    container.NodeGameObjectDatas.Add(goState);
                }
            }

            string path = "Assets/ResourcesRex/SceneContainerData.asset";
            if (File.Exists(path))
            {
                File.Delete(path);
                if (File.Exists($"{path}.meta"))
                {
                    File.Delete($"{path}.meta");
                }

                AssetDatabase.Refresh();
            }

            AssetDatabase.CreateAsset(container, path);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        public void Load(SceneContainer container)
        {
            if(_sceneGraphView == null) return;
            List<BaseNode> list = new List<BaseNode>();
            // 1. 遍历生成 Node 节点
            foreach (SceneScriptable sceneData in container.NodeSceneDatas)
            {
                Vector2 pos = GetPosition(sceneData.Guid, container);
                SceneNode node = _sceneGraphView.CreateNode<SceneNode>(pos, sceneData.Title, sceneData.Guid);
                if (node.State is SceneScriptable data)
                {
                    data.ScenePostion = sceneData.ScenePostion;
                }
                list.Add(node);
            }
            foreach (GroundScriptable groundData in container.NodeGroundDatas)
            {
                Vector2 pos = GetPosition(groundData.Guid, container);
                GroundNode node = _sceneGraphView.CreateNode<GroundNode>(pos, groundData.Title, groundData.Guid);
                list.Add(node);
            }
            foreach (GameObjectScriptable objectData in container.NodeGameObjectDatas)
            {
                Vector2 pos = GetPosition(objectData.Guid, container);
                GameObjectNode node = _sceneGraphView.CreateNode<GameObjectNode>(pos, objectData.Title, objectData.Guid);
                if (node.State is GameObjectScriptable data)
                {
                    data.Position = objectData.Position;
                    data.Rotation = objectData.Rotation;
                    data.Scale = objectData.Scale;
                    data.TemplateGo = objectData.TemplateGo;
                    data.Probability = objectData.Probability;
                    data.ForceStatic = objectData.ForceStatic;
                }

                node.RefreshTempleGo(_sceneGraphView.window);
                list.Add(node);
            }
            
            // 2. 连线
            foreach (SceneNodeLinkData data in container.NodeLinkDatas)
            {
                bool ret = GetEdgeNode(data, list, out BaseNode outputNode, out BaseNode inputNode);
                if (ret)
                {
                    List<Port> inputPorts = inputNode.contentContainer.Query<Port>().ToList();
                    List<Port> outputPorts = outputNode.contentContainer.Query<Port>().ToList();
                    Port inputPort = null;
                    Port outputPort = null;
                    for (int i = 0; i < inputPorts.Count; i++)
                    {
                        if (inputPorts[i].direction == Direction.Input)
                        {
                            inputPort = inputPorts[i];
                        }
                    }
                    for (int i = 0; i < outputPorts.Count; i++)
                    {
                        if (outputPorts[i].direction == Direction.Output)
                        {
                            outputPort = outputPorts[i];
                        }
                    }AddEdgeByPorts(outputPort, inputPort);
                }
            }
        }

        private bool GetEdgeNode(SceneNodeLinkData data, List<BaseNode> list, out BaseNode outputNode, out BaseNode inputNode)
        {
            outputNode = null;
            inputNode = null;
            for (int i = 0; i < list.Count; i++)
            {
                if (list[i].State.Guid == data.OutputNodeGuid)
                {
                    outputNode = list[i];
                }

                if (list[i].State.Guid == data.InputNodeGuid)
                {
                    inputNode = list[i];
                }
            }

            return outputNode != null && inputNode != null;
        }

        private void AddEdgeByPorts(Port outputPort, Port inputPort)
        {
            Edge tempEdge = new Edge()
            {
                output = outputPort,
                input = inputPort
            };
            tempEdge.input.Connect(tempEdge);
            tempEdge.output.Connect(tempEdge);
            _sceneGraphView.Add(tempEdge);
        }

        private Vector2 GetPosition(string guid, SceneContainer container)
        {
            foreach (SceneNodeData data in container.NodeDatas)
            {
                if (data.NodeGuid == guid) return data.NodePosition;
            }

            return Vector2.zero;
        }
    }
}