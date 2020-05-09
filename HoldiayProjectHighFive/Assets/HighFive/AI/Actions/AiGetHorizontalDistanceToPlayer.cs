using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using HighFive.Global;
using ReadyGamerOne.Rougelike.Mover;
using UnityEngine;
using UnityEngine.Assertions;

namespace HighFive.AI.Actions
{
    [TaskDescription("获取当前角色和玩家的水平距离")]
    public class AiGetHorizontalDistanceToPlayer:Action
    {
        [BehaviorDesigner.Runtime.Tasks.Tooltip("和玩家的水平距离差")]
        public SharedFloat outHorzontalDistanceToPlayer;

        private IMover2D selfMover;
        public override void OnAwake()
        {
            base.OnAwake();
            selfMover = gameObject.GetComponent<IMover2D>();
            Assert.IsNotNull(selfMover);
        }

        public override TaskStatus OnUpdate()
        {
            outHorzontalDistanceToPlayer.Value =
                Mathf.Abs(GlobalVar.G_Player.position.x - selfMover.Position.x);
            return TaskStatus.Success;
        }
    }
}