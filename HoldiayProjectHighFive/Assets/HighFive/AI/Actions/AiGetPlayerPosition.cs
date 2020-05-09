using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using HighFive.Global;

namespace HighFive.AI.Actions
{
    
    [TaskDescription("获取玩家位置")]
    public class AiGetPlayerPosition:Action
    {
        [BehaviorDesigner.Runtime.Tasks.Tooltip("获取玩家位置")]
        public SharedVector3 outPlayerPosition;

        public override TaskStatus OnUpdate()
        {
            outPlayerPosition.Value = GlobalVar.G_Player.position;
            return TaskStatus.Success;
        }
    }
}