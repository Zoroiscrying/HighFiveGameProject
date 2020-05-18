using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using HighFive.Control.Movers.Interfaces;
using HighFive.Model.Person;
using ReadyGamerOne.Rougelike.Person;
using ReadyGamerOne.Utility;
using UnityEngine;
using UnityEngine.Assertions;

namespace HighFive.AI.Actions
{
    [TaskDescription("随即飞行函数，恒定返回Running")]
    public class AiRandomFly:Action
    {
        [BehaviorDesigner.Runtime.Tasks.Tooltip("飞行动画名")]
        public SharedString flyAniName;

        private IHighFivePerson self;
        private IFlyActorControl flyMover;
        private Animator ani;

        public override void OnAwake()
        {
            base.OnAwake();
            self=gameObject.GetPersonInfo() as IHighFivePerson;
            Assert.IsNotNull(self);
            flyMover=self.ActorMover as IFlyActorControl;
            Assert.IsNotNull(flyMover);

            ani = gameObject.GetComponent<Animator>();
        }

        public override void OnStart()
        {
            base.OnStart();
            flyMover.StartRandomFly(Vector2.right.RotateDegree(Random.Range(0,360)));
            ani?.Play(Animator.StringToHash(flyAniName.Value));
        }       
        
        public override TaskStatus OnUpdate()
        {
            return TaskStatus.Running;
        }


        public override void OnEnd()
        {
            base.OnEnd();
            Debug.Log("Stop Fly");
            flyMover.StopRandomFly();
//            flyMover.SetMovable(false);
        }
    }
}