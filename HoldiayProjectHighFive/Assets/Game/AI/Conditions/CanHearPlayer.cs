using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using Game.Control.PersonSystem;
using ReadyGamerOne.Global;
using UnityEngine;

namespace Game.AI
{
    public class CanHearPlayer:Conditional
    {
        private AbstractPerson player;
        private AbstractPerson self;

        public SharedFloat hearRadius;
        public LayerMask LayerMask;

        public override void OnAwake()
        {
            base.OnAwake();
            player = GlobalVar.G_Player;
            self = AbstractPerson.GetInstance(gameObject);
            
        }

        public override TaskStatus OnUpdate()
        {
            var dir = player.Pos - self.Pos;
            var hitInfo = Physics2D.Raycast(self.Pos, dir, hearRadius.Value, LayerMask);

            //碰到了Player
            if (hitInfo && hitInfo.transform.gameObject == player.obj)
            {
                return TaskStatus.Success;
            }

            return TaskStatus.Failure;
        }
    
    }
}