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
                Vector2 pos = GetPosition(levelData.Guid, container);
                BaseNode node = _sceneGraphView.CreateNode(typeof(LevelDataNode), pos, levelData.Title, levelData.Guid);
                if (node is LevelDataNode groundNode)
                {
                    if (node.State is LevelDataScriptable levelDataScriptable)
                    {
                        levelDataScriptable.LevelName = levelData.LevelName;
                    }

                    list.Add(groundNode);
                }
            }

            foreach (BoxTriggerScriptable boxData in container.BoxTriggerDatas)
            {
                Vector2 pos = GetPosition(boxData.Guid, container);
                BaseNode node = _sceneGraphView.CreateNode(typeof(BoxTriggerNode), pos, boxData.Title, boxData.Guid);
                if (node is BoxTriggerNode boxTriggerNode)
                {
                    if (node.State is BoxTriggerScriptable boxTriggerScriptable)
                    {
                        boxTriggerScriptable.Position = boxData.Position;
                        boxTriggerScriptable.Scale = boxData.Scale;
                        boxTriggerScriptable.TriggerState = boxData.TriggerState;
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

        private Vector2 GetPosition(string guid, LevelTriggerContainer container)
        {
            foreach (SceneNodeData data in container.NodeDatas)
            {
                if (data.NodeGuid == guid) return data.NodePosition;
            }

            return Vector2.zero;
        }

        public string Save(string path)
        {
            if (!edges.Any()) return path;

            LevelTriggerContainer container = ScriptableObject.CreateInstance<LevelTriggerContainer>();

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

                if (node.State is LevelDataScriptable levelDataNodeState)
                {
                    container.LevelDatas.Add(levelDataNodeState);
                }
                else if (node.State is BoxTriggerScriptable boxTriggerScriptable)
                {
                    container.BoxTriggerDatas.Add(boxTriggerScriptable);
                }
            }

            string savePath = path;
            if (string.IsNullOrEmpty(savePath))
            {
                string filePath = EditorUtility.SaveFilePanel("选择文件", Application.dataPath, "LevelTriggerContainer", "asset");
                if (!string.IsNullOrEmpty(filePath))
                {
                    savePath = filePath.Substring(Application.dataPath.Length - 6);
                }
            }

            FileExistAndDelete(savePath);
            AssetDatabase.CreateAsset(container, savePath);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            return savePath;
        }

        private void FileExistAndDelete(string path)
        {
            if (!string.IsNullOrEmpty(path) && File.Exists(path))
            {
                File.Delete(path);
                if (File.Exists($"{path}.meta"))
                {
                    File.Delete($"{path}.meta");
                }

                AssetDatabase.Refresh();
            }
        }
    }
}