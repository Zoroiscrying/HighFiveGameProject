using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using HighFive.Global;

namespace HighFive.AI.Actions
{
    [TaskDescription("获取玩家在当前角色哪个方向")]
    public class AiGetDirToPlayer:Action
    {
        [BehaviorDesigner.Runtime.Tasks.Tooltip("玩家在当前角色哪个方向")]
        public SharedInt outDirToPlayer;

        public override TaskStatus OnUpdate()
        {
            outDirToPlayer.Value = GlobalVar.G_Player.position.x > transform.position.x ? 1 : -1;
            return TaskStatus.Success;
        }
    }
}