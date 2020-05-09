using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using HighFive.Global;
using UnityEngine;

namespace HighFive.AI.Actions
{
    [TaskDescription("获取当前角色和玩家的水平距离")]
    public class AiGetHorizontalDistanceToPlayer:Action
    {
        [BehaviorDesigner.Runtime.Tasks.Tooltip("和玩家的水平距离差")]
        public SharedFloat outHorzontalDistanceToPlayer;

        public override TaskStatus OnUpdate()
        {
            outHorzontalDistanceToPlayer.Value =
                Mathf.Abs(GlobalVar.G_Player.position.x - transform.position.x);
            return TaskStatus.Success;
        }
    }
}