using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using HighFive.Global;
using ReadyGamerOne.Rougelike.Mover;
using UnityEngine.Assertions;

namespace HighFive.AI.Actions
{
    [TaskDescription("获取玩家在当前角色哪个方向")]
    public class AiGetDirToPlayer:Action
    {
        [BehaviorDesigner.Runtime.Tasks.Tooltip("玩家在当前角色哪个方向")]
        public SharedInt outDirToPlayer;

        private IMover2D selfMover;
        public override void OnAwake()
        {
            base.OnAwake();
            selfMover = gameObject.GetComponent<IMover2D>();
            Assert.IsNotNull(selfMover);
        }

        public override TaskStatus OnUpdate()
        {
            outDirToPlayer.Value = GlobalVar.G_Player.position.x > selfMover.Position.x ? 1 : -1;
            return TaskStatus.Success;
        }
    }
}