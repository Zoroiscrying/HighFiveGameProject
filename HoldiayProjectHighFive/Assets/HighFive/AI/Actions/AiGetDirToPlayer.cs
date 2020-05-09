using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using HighFive.Global;
using HighFive.Model.Person;
using ReadyGamerOne.Rougelike.Mover;
using ReadyGamerOne.Rougelike.Person;
using UnityEngine.Assertions;

namespace HighFive.AI.Actions
{
    [TaskDescription("获取玩家在当前角色哪个方向")]
    public class AiGetDirToPlayer:Action
    {
        [BehaviorDesigner.Runtime.Tasks.Tooltip("玩家在当前角色哪个方向")]
        public SharedInt outDirToPlayer;

        private IHighFivePerson selfPerson;
        public override void OnAwake()
        {
            base.OnAwake();
            selfPerson = gameObject.GetPersonInfo() as IHighFivePerson;
            Assert.IsNotNull(selfPerson);
        }

        public override TaskStatus OnUpdate()
        {
            outDirToPlayer.Value = GlobalVar.G_Player.position.x > selfPerson.position.x ? 1 : -1;
            return TaskStatus.Success;
        }
    }
}