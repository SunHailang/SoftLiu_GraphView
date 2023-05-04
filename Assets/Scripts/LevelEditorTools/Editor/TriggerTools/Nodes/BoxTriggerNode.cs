using UnityEditor.Experimental.GraphView;

namespace LevelEditorTools.Editor.Nodes
{
    public class BoxTriggerNode : BaseTriggerNode
    {
        public BoxTriggerNode()
        {
            Port input = GraphViewUtils.GetInstantiatePort(this, Orientation.Vertical, Direction.Input, Port.Capacity.Single, typeof(int));
            this.inputContainer.Add(input);
            Port output = GraphViewUtils.GetInstantiatePort(this, Orientation.Vertical, Direction.Output, Port.Capacity.Single, typeof(int));
            this.outputContainer.Add(output);
            this.RefreshExpandedState();
            this.RefreshPorts();
        }
    }
}