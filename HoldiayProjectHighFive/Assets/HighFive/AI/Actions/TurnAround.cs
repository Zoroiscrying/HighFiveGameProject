using BehaviorDesigner.Runtime.Tasks;
using HighFive.Model.Person;
using ReadyGamerOne.Rougelike.Person;
using UnityEngine;

namespace HighFive.AI.Actions
{
    public class TurnAround:Action
    {
        private IHighFivePerson self;
        public override void OnStart()
        {
            base.OnStart();
            self = gameObject.GetPersonInfo() as IHighFivePerson;
        }

        public override TaskStatus OnUpdate()
        {
            self.Dir *= -1;
            return TaskStatus.Success;
        }

        public override void OnEnd()
        {
            Debug.Log("掉头");
        }
    }
}