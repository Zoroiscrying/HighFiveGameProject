using System;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using DG.Tweening;
using HighFive.Control.SkillSystem;
using ReadyGamerOne.Rougelike.Mover;
using UnityEngine;
using UnityEngine.Assertions;
using Action = BehaviorDesigner.Runtime.Tasks.Action;

namespace HighFive.AI.Actions
{
    public class AiJumToGameObject:Action
    {
        public SharedGameObject inTarget;
        public SharedFloat inTime;
        public SharedVector3 inOffset;

        public SharedVector3 outTargetVector3;
        public Ease easeType;

        private bool finished = false;

        private IMover2D selfMover;
        public override void OnAwake()
        {
            base.OnAwake();
            selfMover = gameObject.GetComponent<IMover2D>();
//            if (null == outTargetVector3)
//                throw new Exception("outTargetVector3 is null");
//            if (null == inTarget)
//                throw new Exception("inTarget is null");
//            if (null == inTarget.Value)
//                throw new Exception("inTarget.Value is null");
            Assert.IsNotNull(selfMover);
        }

        public override void OnStart()
        {
            base.OnStart();
            finished = false;
            outTargetVector3.Value = inTarget.Value.transform.position;
            DOTween.To(
                    () => selfMover.Position,
                    value => selfMover.Position = value,
                    outTargetVector3.Value+inOffset.Value,
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