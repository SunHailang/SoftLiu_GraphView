using UnityEditor.Experimental.GraphView;

namespace GraphEditor.LevelTrigger
{
    public class ConditionTriggerNode : BaseTriggerNode
    {
        public ConditionTriggerNode()
        {
            Port output = GraphViewUtils.GetInstantiatePort(this, Orientation.Vertical, Direction.Output, Port.Capacity.Single, typeof(int));
            this.outputContainer.Add(output);
            
            this.RefreshExpandedState();
            this.RefreshPorts();
        }
    }
}