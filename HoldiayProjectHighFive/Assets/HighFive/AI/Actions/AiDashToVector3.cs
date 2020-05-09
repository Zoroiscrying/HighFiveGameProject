using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using DG.Tweening;
using ReadyGamerOne.Rougelike.Mover;
using UnityEngine;
using UnityEngine.Assertions;

namespace HighFive.AI.Actions
{    
    [TaskDescription("冲锋到一个【Vector3+offset】")]
    public class AiDashToVector3:Action
    {
        [BehaviorDesigner.Runtime.Tasks.Tooltip("瞬移到什么位置")]
        public SharedVector3 inTarget;
        
        [BehaviorDesigner.Runtime.Tasks.Tooltip("瞬移时间")]
        public SharedFloat inTime;
        [BehaviorDesigner.Runtime.Tasks.Tooltip("偏移")]
        public SharedVector3 inOffset;
        
        [BehaviorDesigner.Runtime.Tasks.Tooltip("缓动类型")]
        public Ease easeType;
        
        
        [BehaviorDesigner.Runtime.Tasks.Tooltip("是否不进行Y向移动")]
        public SharedBool ignoreYAxis = false;

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
            if (ignoreYAxis.Value)
                targetPos.y = selfMover.Position.y;
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