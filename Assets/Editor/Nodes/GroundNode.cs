using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace Editor.Nodes
{
    /// <summary>
    /// 地形 Node 用于创建地形 所以 一个输入端口可以允许多个输入
    /// </summary>
    public class GroundNode : BaseNode
    {
        public GroundNode()
        {
            // 创建输入
            Port input = GraphViewUtils.GetInstantiatePort(this, Orientation.Horizontal, Direction.Input, Port.Capacity.Multi, typeof(int));
            input.portName = "input";
            input.portColor = Color.blue;
            this.Add(input);
            
            Port output = GraphViewUtils.GetInstantiatePort(this, Orientation.Horizontal, Direction.Output, Port.Capacity.Single, typeof(int));
            input.portName = "output";
            input.portColor = Color.green;
            this.Add(input);
        }
    
    
    }
}