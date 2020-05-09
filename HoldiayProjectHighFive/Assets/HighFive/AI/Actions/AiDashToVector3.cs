using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using DG.Tweening;
using HighFive.Model.Person;
using ReadyGamerOne.Rougelike.Mover;
using ReadyGamerOne.Rougelike.Person;
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

        private IHighFivePerson selfPerson;
        public override void OnAwake()
        {
            base.OnAwake();
            selfPerson = gameObject.GetPersonInfo() as IHighFivePerson;
            Assert.IsNotNull(selfPerson);
        }

        public override void OnStart()
        {
            base.OnStart();
            finished = false; 
            targetPos = inTarget.Value+inOffset.Value;
            if (ignoreYAxis.Value)
                targetPos.y = selfPerson.position.y;
            DOTween.To(
                    () => selfPerson.position,
                    value => selfPerson.position = value,
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