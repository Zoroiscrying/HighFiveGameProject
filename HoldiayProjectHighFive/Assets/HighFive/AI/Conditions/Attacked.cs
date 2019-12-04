using BehaviorDesigner.Runtime.Tasks;
using HighFive.Const;
using ReadyGamerOne.Common;
using UnityEngine;

namespace HighFive.AI.Conditions
{
    public class Attacked:Conditional
    {
        private bool attacked = false;

        private void OnAttacked(int damage)
        {
//            Debug.Log("救命，有人打我");
            attacked = true;
        }
        public override void OnAwake()
        {
            base.OnStart();
            attacked = false;
//            Debug.Log("监听挨打");
            CEventCenter.AddListener<int>(Message.M_BloodChange(gameObject), OnAttacked);
        }

        public override void OnBehaviorComplete()
        {
            base.OnBehaviorComplete();
            Debug.Log("不再监听挨打");
            CEventCenter.RemoveListener<int>(Message.M_BloodChange(gameObject), OnAttacked);
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