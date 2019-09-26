using BehaviorDesigner.Runtime.Tasks;
using Game.Const;
using Game.Control.PersonSystem;
using ReadyGamerOne.Common;

namespace Game.AI
{
    public class Attacked:Conditional
    {
        private bool attacked = false;

        private void OnAttacked(int damage)
        {
            attacked = true;
        }
        private AbstractPerson self;
        public override void OnStart()
        {
            base.OnStart();
            attacked = false;
            self = AbstractPerson.GetInstance(gameObject);
            CEventCenter.AddListener<int>(Message.M_BloodChange(gameObject), OnAttacked);
        }

        public override void OnEnd()
        {
            base.OnEnd();
            CEventCenter.RemoveListener<int>(Message.M_BloodChange(gameObject), OnAttacked);
        }

        public override TaskStatus OnUpdate()
        {
            if (attacked)
                return TaskStatus.Success;

            return TaskStatus.Failure;
        }
    }
}