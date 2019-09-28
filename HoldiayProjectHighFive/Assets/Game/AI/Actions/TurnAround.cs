using BehaviorDesigner.Runtime.Tasks;
using Game.Control.PersonSystem;
using UnityEngine;

namespace Game.AI
{
    public class TurnAround:Action
    {
        private AbstractPerson self;
        public override void OnStart()
        {
            base.OnStart();
            self = AbstractPerson.GetInstance(gameObject);
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