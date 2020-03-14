using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using HighFive.Model.Person;
using ReadyGamerOne.Rougelike.Person;
using UnityEngine;

namespace HighFive.AI.Conditions
{
    public class CanSeePlayer:Conditional
    {
        public SharedFloat detectDistance;
        public LayerMask LayerMask;

        private IHighFivePerson self;
        public override void OnStart()
        {
            base.OnStart();
            self = gameObject.GetPersonInfo() as IHighFivePerson;
        }

        public override TaskStatus OnUpdate()
        {
            
            var hit= Physics2D.Linecast(gameObject.transform.position,
                gameObject.transform.position + new Vector3(self.Dir * detectDistance.Value, 0,0), LayerMask);
            
            
            if (hit && hit.transform.gameObject.GetPersonInfo() is IHighFiveCharacter)
            {
                
//                Debug.Log("在检测范围内，开始追击");
                return TaskStatus.Success;
            }

            return TaskStatus.Failure;
        }

        public override void OnDrawGizmos()
        {
            Gizmos.DrawLine(transform.position,transform.position + new Vector3(self.Dir * detectDistance.Value, 0,0));
        }
    }
}