using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using DG.Tweening;
using ReadyGamerOne.Rougelike.Mover;
using UnityEngine;
using UnityEngine.Assertions;

namespace HighFive.AI.Actions
{
    public class AiGetLandingPoint:Action
    {
        public SharedVector3 outLandingPoint;
        public SharedVector3 inOffset;

        private IMover2D selfMover;

        public override void OnAwake()
        {
            base.OnAwake();
            selfMover = gameObject.GetComponent<IMover2D>();
            Assert.IsNotNull(selfMover);
        }
        
        public override TaskStatus OnUpdate()
        {
            var hitInfo = Physics2D.Raycast(
                selfMover.Position,
                Vector2.down,
                100,
                selfMover.ColliderLayers);
            if (hitInfo.collider == null)
                return TaskStatus.Failure;

            outLandingPoint.Value = hitInfo.point;
            outLandingPoint.Value += inOffset.Value;
            return TaskStatus.Success;
        }
    }
}