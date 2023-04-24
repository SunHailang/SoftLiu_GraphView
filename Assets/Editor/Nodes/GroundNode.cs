using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace GraphEditor.Nodes
{
    /// <summary>
    /// 地形 Node 用于创建地形 所以 一个输入端口可以允许多个输入
    /// </summary>
    public class GroundNode : BaseNode
    {
        public GroundNode()
        {
            // 创建输入
            Port input = GraphViewUtils.GetInstantiatePort(this, Orientation.Vertical, Direction.Input, Port.Capacity.Single, typeof(int));
            this.Add(input);

            Port output = GraphViewUtils.GetInstantiatePort(this, Orientation.Vertical, Direction.Output, Port.Capacity.Multi, typeof(int));
            this.Add(output);

            State = new GroundScriptable();
        }
    }
}