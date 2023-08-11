using System;
using System.Collections.Generic;
using LevelEditorTools;
using LevelEditorTools.Editor.Nodes;
using Unity.CodeEditor;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace EditorUtils
{
    public static class GraphViewUtils
    {
        public static Port GetInstantiatePort(Node node,
            Orientation orientation,
            Direction direction,
            Port.Capacity capacity,
            System.Type type)
        {
            Port port = node.InstantiatePort(orientation, direction, capacity, type);
            port.portName = direction.ToString();
            port.portColor = direction == Direction.Input ? Color.blue : Color.green;
            return port;
        }

        public static Vector2 GetNodePosition(string guid, List<SceneNodeData> list)
        {
            foreach (SceneNodeData data in list)
            {
                if (data.NodeGuid == guid) return data.NodePosition;
            }

            return Vector2.zero;
        }

        public static bool GetEdgeNode(SceneNodeLinkData data, List<BaseNode> list, out BaseNode outputNode, out BaseNode inputNode)
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

        public static void AddEdgeByPorts(GraphView graphView, Port outputPort, Port inputPort)
        {
            Edge tempEdge = new Edge()
            {
                output = outputPort,
                input = inputPort
            };
            tempEdge.input.Connect(tempEdge);
            tempEdge.output.Connect(tempEdge);
            graphView.Add(tempEdge);
        }

        public static void OpenCodeEditor(string scriptName)
        {
            string[] paths = AssetDatabase.FindAssets($"t:Script {scriptName}");
            if (paths.Length > 0)
            {
                Queue<string> scriptPaths = new Queue<string>();
                foreach (string path in paths)
                {
                    string data = AssetDatabase.GUIDToAssetPath(path);
                    string script = $"{scriptName}.cs";
                    int index = data.LastIndexOf(script, StringComparison.Ordinal);
                    if (index > 0)
                    {
                        string substring = data.Substring(index);
                        if (script == substring)
                        {
                            scriptPaths.Enqueue(data);
                        }
                    }
                }

                if (scriptPaths.Count > 1)
                {
                    while(scriptPaths.TryDequeue(out string scriptPath))
                    {
                        Debug.Log($"Path: {scriptPath}");
                    }
                    Debug.LogError($"Error: EditorScriptContextMenu More One {scriptName} Script.");
                }
                else if (scriptPaths.TryDequeue(out string scriptPath))
                {
                    CodeEditor.CurrentEditor.OpenProject(scriptPath);
                }
            }
            else
            {
                Debug.LogError("Error: EditorScriptContextMenu Not Found SceneBezierNode Script.");
            }
        }

        public static Vector3 GetPostion(Matrix4x4 matrix)
        {
            return matrix.GetPosition();
        }

        public static Vector3 GetScale(Matrix4x4 matrix)
        {
            return matrix.lossyScale;
        }

        public static Quaternion GetRotation(Matrix4x4 matrix)
        {
            return matrix.rotation;
        }

        public static Matrix4x4 SetMatrix4x4(Vector3 postion, Quaternion rotion, Vector3 scale)
        {
            return Matrix4x4.TRS(postion, rotion, scale);
        }
    }
}