using BehaviorDesigner.Runtime.Tasks;
using ReadyGamerOne.Rougelike.Person;

namespace HighFive.AI.Conditions
{
    public class Attacked:Conditional
    {
        private bool attacked = false;

        private void OnAttacked(AbstractPerson takeFromWho, int damage)
        {
//            Debug.Log("救命，有人打我");
            attacked = true;
        }
        public override void OnAwake()
        {
            base.OnStart();
            attacked = false;
//            Debug.Log("监听挨打");
            gameObject.GetPersonInfo().onTakeDamage += OnAttacked;
        }

        public override void OnBehaviorComplete()
        {
            base.OnBehaviorComplete();
//            Debug.Log("不再监听挨打");
            gameObject.GetPersonInfo().onTakeDamage -= OnAttacked;
        }
        

        public override TaskStatus OnUpdate()
        {
            if (attacked)
            {
                attacked = false;
                return TaskStatus.Success;
            }

            return TaskStatus.Failure;
        }
    }
}