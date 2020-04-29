using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using HighFive.Global;
using HighFive.Model.Person;
using ReadyGamerOne.Rougelike.Person;
using UnityEngine;

namespace HighFive.AI.Conditions
{
    public class CanHearPlayer:Conditional
    {
        private IHighFivePerson player;
        private IHighFivePerson self;

        public SharedFloat hearRadius;
        public LayerMask LayerMask;

        public override void OnAwake()
        {
            base.OnAwake();
            player = GlobalVar.G_Player as IHighFivePerson;
            self = gameObject.GetPersonInfo() as IHighFivePerson;
            
        }

        public override TaskStatus OnUpdate()
        {
            var dir = player.position - self.position;
            var hitInfo = Physics2D.Raycast(self.position, dir, hearRadius.Value, LayerMask);

            //碰到了Player
            if (hitInfo && hitInfo.transform.gameObject == player.gameObject)
            {
                return TaskStatus.Success;
            }

            return TaskStatus.Failure;
        }
    
    }
}