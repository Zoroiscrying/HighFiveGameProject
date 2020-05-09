using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using HighFive.Control.Movers.Interfaces;
using HighFive.Model.Person;
using ReadyGamerOne.Rougelike.Person;
using UnityEngine;
using UnityEngine.Assertions;

namespace HighFive.AI.Actions
{
	[TaskDescription("巡逻函数，恒定返回Running")]
	public class AiPatrol : Action 
	{
		public SharedString walkAniName;

        public SharedFloat waitTimeWhenStop;
        public SharedFloat distance = float.MaxValue;
        public SharedBool loopPatrol = false;
        
        private IHighFivePerson self;
        private IActorMoveAIControl aiMover;
        
        private Animator ani;

        public override void OnAwake()
        {
	        base.OnAwake();
	        self = gameObject.GetPersonInfo() as IHighFivePerson;
	        Assert.IsNotNull(self);
	        aiMover=self.ActorMover as IActorMoveAIControl;
	        Assert.IsNotNull(aiMover);
	        ani = gameObject.GetComponent<Animator>();
	        Assert.IsTrue(ani);
        }

        public override void OnStart()
        {
	        base.OnStart();
	        if(!string.IsNullOrEmpty(walkAniName.Value))
				ani.Play(Animator.StringToHash(walkAniName.Value));
	        aiMover.StartPatrolling(
		        self.Dir * Vector2.right,
		        waitTimeWhenStop.Value,
		        distance.Value,
		        loopPatrol.Value);
        }

        public override TaskStatus OnUpdate()
        {
	        return TaskStatus.Running;
        }

        public override void OnEnd()
        {
	        base.OnEnd();
	        aiMover.StopPatrolling();
        }
	}

}

