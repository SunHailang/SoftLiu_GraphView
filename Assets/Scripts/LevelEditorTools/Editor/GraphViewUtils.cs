using System.Collections.Generic;
using LevelEditorTools.Editor.Nodes;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace LevelEditorTools
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