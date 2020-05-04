using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using DG.Tweening;
using ReadyGamerOne.Rougelike.Mover;
using UnityEngine;
using UnityEngine.Assertions;

namespace HighFive.AI.Actions
{
    public class AiJumpToVector3:Action
    {
        public SharedVector3 inTarget;
        public SharedFloat inTime;
        public SharedVector3 inOffset;
        
        public Ease easeType;

        private bool finished = false;
        private Vector3 targetPos;

        private IMover2D selfMover;
        public override void OnAwake()
        {
            base.OnAwake();
            selfMover = gameObject.GetComponent<IMover2D>();
            Assert.IsNotNull(selfMover);
        }

        public override void OnStart()
        {
            base.OnStart();
            finished = false; 
            targetPos = inTarget.Value+inOffset.Value;
            DOTween.To(
                    () => selfMover.Position,
                    value => selfMover.Position = value,
                    targetPos,
                    inTime.Value)
                .SetEase(easeType)
                .onComplete = () => { finished = true; };
        }
        
        

        public override TaskStatus OnUpdate()
        {
            return finished ? TaskStatus.Success : TaskStatus.Running;
        }
    }
}