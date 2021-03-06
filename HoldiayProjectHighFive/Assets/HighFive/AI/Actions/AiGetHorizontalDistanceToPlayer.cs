using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using HighFive.Global;
using HighFive.Model.Person;
using ReadyGamerOne.Rougelike.Mover;
using ReadyGamerOne.Rougelike.Person;
using UnityEngine;
using UnityEngine.Assertions;

namespace HighFive.AI.Actions
{
    [TaskDescription("获取当前角色和玩家的水平距离")]
    public class AiGetHorizontalDistanceToPlayer:Action
    {
        [BehaviorDesigner.Runtime.Tasks.Tooltip("和玩家的水平距离差")]
        public SharedFloat outHorzontalDistanceToPlayer;

        private IHighFivePerson selfPerson;
        public override void OnAwake()
        {
            base.OnAwake();
            selfPerson = gameObject.GetPersonInfo() as IHighFivePerson;
            Assert.IsNotNull(selfPerson);
        }

        public override TaskStatus OnUpdate()
        {
            outHorzontalDistanceToPlayer.Value =
                Mathf.Abs(GlobalVar.G_Player.position.x - selfPerson.position.x);
            return TaskStatus.Success;
        }
    }
}