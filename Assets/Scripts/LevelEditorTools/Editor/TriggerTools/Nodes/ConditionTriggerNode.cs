using LevelEditorTools.Nodes;
using UnityEditor.Experimental.GraphView;

namespace LevelEditorTools.Editor.Nodes
{
    public class ConditionTriggerNode : BaseNode
    {
        public ConditionTriggerNode()
        {
            Port output = GraphViewUtils.GetInstantiatePort(this, Orientation.Vertical, Direction.Output, Port.Capacity.Single, typeof(int));
            this.outputContainer.Add(output);
            
            this.RefreshExpandedState();
            this.RefreshPorts();

            State = new ConditionTriggerScriptable();
        }
    }
}