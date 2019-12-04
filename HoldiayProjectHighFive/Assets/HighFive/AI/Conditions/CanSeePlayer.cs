using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using HighFive.Control.PersonSystem.Persons;
using UnityEngine;

namespace HighFive.AI.Conditions
{
    public class CanSeePlayer:Conditional
    {
        public SharedFloat detectDistance;
        public LayerMask LayerMask;

        private AbstractPerson self;
        public override void OnStart()
        {
            base.OnStart();
            self = AbstractPerson.GetInstance(gameObject);
        }

        public override TaskStatus OnUpdate()
        {
            
            var hit= Physics2D.Linecast(gameObject.transform.position,
                gameObject.transform.position + new Vector3(self.Dir * detectDistance.Value, 0,0), LayerMask);
            
            
            if (hit && AbstractPerson.GetInstance(hit.transform.gameObject) is Player)
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