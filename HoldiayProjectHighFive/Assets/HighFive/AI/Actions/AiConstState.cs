using BehaviorDesigner.Runtime.Tasks;

namespace HighFive.AI.Actions
{
    [TaskDescription("始终返回制定的状态")]

    public class AiConstState:Action
    {
        [BehaviorDesigner.Runtime.Tasks.Tooltip("恒定返回的状态")]
        public TaskStatus status;
        public override TaskStatus OnUpdate()
        {
            return status;
        }
    }
}