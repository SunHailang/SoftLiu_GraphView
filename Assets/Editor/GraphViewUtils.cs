using UnityEditor.Experimental.GraphView;

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
            return node.InstantiatePort(orientation, direction, capacity, type);
        }
    }
}