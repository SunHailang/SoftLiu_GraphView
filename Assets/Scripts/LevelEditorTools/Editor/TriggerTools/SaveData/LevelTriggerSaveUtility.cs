using System.Collections.Generic;
using System.IO;
using System.Linq;
using LevelEditorTools.Editor.Nodes;
using LevelEditorTools.GraphViews;
using LevelEditorTools.Nodes;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

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
            if (_sceneGraphView == null) return;
            List<BaseNode> list = new List<BaseNode>();
            // 1. 遍历生成 Node 节点
            foreach (LevelDataScriptable levelData in container.LevelDatas)
            {
                Vector2 pos = GraphViewUtils.GetNodePosition(levelData.Guid, container.NodeDatas);
                BaseNode node = _sceneGraphView.CreateNode(typeof(LevelDataNode), pos, levelData.Title, levelData.Guid);
                if (node is LevelDataNode groundNode)
                {
                    if (node.State is LevelDataScriptable levelDataScriptable)
                    {
                        levelDataScriptable.LevelName = levelData.LevelName;
                        levelDataScriptable.LevelPosition = levelData.LevelPosition;
                        levelDataScriptable.LevelScale = levelData.LevelScale;
                    }

                    list.Add(groundNode);
                }
            }

            foreach (BoxTriggerScriptable boxData in container.BoxTriggerDatas)
            {
                Vector2 pos = GraphViewUtils.GetNodePosition(boxData.Guid, container.NodeDatas);
                BaseNode node = _sceneGraphView.CreateNode(typeof(BoxTriggerNode), pos, boxData.Title, boxData.Guid);
                if (node is BoxTriggerNode boxTriggerNode)
                {
                    if (node.State is BoxTriggerScriptable boxTriggerScriptable)
                    {
                        boxTriggerScriptable.EventID = boxData.EventID;
                        boxTriggerScriptable.Position = boxData.Position;
                        boxTriggerScriptable.Scale = boxData.Scale;
                        boxTriggerScriptable.TriggerState = boxData.TriggerState;
                        boxTriggerScriptable.IsOnce = boxData.IsOnce;
                        boxTriggerScriptable.EnemyPosition = boxData.EnemyPosition;
                    }

                    list.Add(boxTriggerNode);
                }
            }

            foreach (ConditionTriggerScriptable conditionTriggerData in container.ConditionTriggerDatas)
            {
                Vector2 pos = GraphViewUtils.GetNodePosition(conditionTriggerData.Guid, container.NodeDatas);
                BaseNode node = _sceneGraphView.CreateNode(typeof(ConditionTriggerNode), pos, conditionTriggerData.Title, conditionTriggerData.Guid);
                if (node is ConditionTriggerNode boxTriggerNode)
                {
                    if (node.State is ConditionTriggerScriptable conditionTrigger)
                    {
                    }

                    list.Add(boxTriggerNode);
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

        public string Save(string path)
        {
            // 单个节点也可以保存
            //if (!edges.Any()) return path;

            LevelTriggerContainer container = AssetDatabase.LoadAssetAtPath<LevelTriggerContainer>(path);
            bool newFile = container == null;
            container ??= ScriptableObject.CreateInstance<LevelTriggerContainer>();

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
            container.LevelDatas.Clear();
            container.BoxTriggerDatas.Clear();
            container.ConditionTriggerDatas.Clear();
            foreach (BaseNode node in nodes)
            {
                SceneNodeData nodeData = new SceneNodeData()
                {
                    NodeGuid = node.State.Guid,
                    NodePosition = node.GetPosition().position
                };
                container.NodeDatas.Add(nodeData);

                if (node.State is LevelDataScriptable levelDataNodeState)
                {
                    container.LevelDatas.Add(levelDataNodeState);
                }
                else if (node.State is BoxTriggerScriptable boxTriggerScriptable)
                {
                    container.BoxTriggerDatas.Add(boxTriggerScriptable);
                }
                else if (node.State is ConditionTriggerScriptable conditionTriggerScriptable)
                {
                    container.ConditionTriggerDatas.Add(conditionTriggerScriptable);
                }
            }

            if (newFile)
            {
                string filePath = EditorUtility.SaveFilePanel("选择文件", Application.dataPath, "LevelTriggerContainer", "asset");
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
    }
}