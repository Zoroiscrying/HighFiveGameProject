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
		[BehaviorDesigner.Runtime.Tasks.Tooltip("移动动画名")]
		public SharedString walkAniName;

		[BehaviorDesigner.Runtime.Tasks.Tooltip("走路停下等待时间")]
        public SharedFloat waitTimeWhenStop;
        [BehaviorDesigner.Runtime.Tasks.Tooltip("最大巡逻距离")]
        public SharedFloat distance = float.MaxValue;
        [BehaviorDesigner.Runtime.Tasks.Tooltip("巡逻是否循环")]
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
        }

        public override void OnStart()
        {
	        base.OnStart();
	        if(!string.IsNullOrEmpty(walkAniName.Value))
				ani?.Play(Animator.StringToHash(walkAniName.Value));
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

