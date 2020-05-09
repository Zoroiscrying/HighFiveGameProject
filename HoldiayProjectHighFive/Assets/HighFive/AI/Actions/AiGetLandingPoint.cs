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
    
    [TaskDescription("从当前位置向下发射一条射线，获取【落地点+offset】")]
    public class AiGetLandingPoint:Action
    {
        [BehaviorDesigner.Runtime.Tasks.Tooltip("当前位置落地点")]
        public SharedVector3 outLandingPoint;
        [BehaviorDesigner.Runtime.Tasks.Tooltip("接触点偏移")]
        public SharedVector3 inOffset;

        private IHighFivePerson selfPerson;

        public override void OnAwake()
        {
            base.OnAwake();
            selfPerson = gameObject.GetPersonInfo() as IHighFivePerson;
            Assert.IsNotNull(selfPerson);
        }
        
        public override TaskStatus OnUpdate()
        {
            var hitInfo = Physics2D.Raycast(
                selfPerson.position,
                Vector2.down,
                100,
                selfPerson.ActorMover.ColliderLayers);
            if (hitInfo.collider == null)
                return TaskStatus.Failure;

            outLandingPoint.Value = hitInfo.point;
            outLandingPoint.Value += inOffset.Value;
            return TaskStatus.Success;
        }
    }
}