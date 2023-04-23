using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace Editor
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
            port.portName = nameof(direction);
            port.portColor = direction == Direction.Input ? Color.blue : Color.green;
            return port;
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