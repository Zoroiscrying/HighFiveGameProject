using BehaviorDesigner.Runtime.Tasks;

namespace HighFive.AI.Actions
{
    [TaskDescription("始终返回制定的状态")]

    public class AiConstState:Action
    {
        public TaskStatus status;
        public override TaskStatus OnUpdate()
        {
            return status;
        }
    }
}