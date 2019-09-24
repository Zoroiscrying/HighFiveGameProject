using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using ReadyGamerOne.Global;
using UnityEngine;

namespace Game.AI
{
    public class CanSeePlayer:Conditional
    {
        public SharedFloat detectDistance;
        public SharedFloat falutReloranceHeight;
        private float disSqr;
        public override void OnAwake()
        {
            base.OnAwake();
            disSqr = detectDistance.Value * detectDistance.Value;
        }

        public override TaskStatus OnUpdate()
        {
            if (Vector2.Distance(GlobalVar.G_Player.Pos, gameObject.transform.position) < disSqr
                && Mathf.Abs(GlobalVar.G_Player.Pos.y - gameObject.transform.position.y) < falutReloranceHeight.Value)
            {
                return TaskStatus.Success;
            }

            return TaskStatus.Failure;
        }
    }
}