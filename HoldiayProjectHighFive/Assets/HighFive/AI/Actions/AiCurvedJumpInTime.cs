using System.Linq;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using HighFive.Model.Person;
using ReadyGamerOne.Rougelike.Mover;
using ReadyGamerOne.Rougelike.Person;
using ReadyGamerOne.ScriptableObjects;
using UnityEngine;
using UnityEngine.Assertions;

namespace HighFive.AI.Actions
{
    [TaskDescription("使用曲线控制Y向速度的方式，在指定时间内跳跃到指定位置")]
    public class AiCurvedJumpInTime:Action
    {
        [BehaviorDesigner.Runtime.Tasks.Tooltip("跳到那个位置")]
        public SharedVector3 inTargetPosition;
        [BehaviorDesigner.Runtime.Tasks.Tooltip("几秒之内跳完")]
        public SharedFloat expectedTime;
        [BehaviorDesigner.Runtime.Tasks.Tooltip("跳跃高度缩放")]
        public SharedFloat yScale = 1.0f;
        [BehaviorDesigner.Runtime.Tasks.Tooltip("使用的跳跃曲线资源")]
        public AnimationCurveAsset aniY;

        public SharedString jumpAniName;

        private float playSpeed;
        private float xSpeed = 1.0f;
        private float animationTime;
        private IHighFivePerson selfPerson;

        public override void OnAwake()
        {
            base.OnAwake();
            selfPerson = gameObject.GetPersonInfo() as IHighFivePerson;
            Assert.IsNotNull(selfPerson);
            this.animationTime = aniY.curve.keys.Last().time;
        }

        private float timer = 0;
        private int dir = 0;
        public override void OnStart()
        {
            base.OnStart();

            var targetPosition = inTargetPosition.Value;
            var selfPosition = selfPerson.position;
            var xDis = Mathf.Abs(targetPosition.x - selfPosition.x);
            
            xSpeed = xDis / expectedTime.Value;
            playSpeed = animationTime / expectedTime.Value;
            
            
            timer = 0;
            dir = targetPosition.x > selfPosition.x ? 1 : -1;

            if(!string.IsNullOrEmpty(jumpAniName.Value))
            {
                GetComponent<Animator>()?.Play(Animator.StringToHash(jumpAniName.Value));
            }
        }

        public override void OnFixedUpdate()
        {
            base.OnFixedUpdate();
            selfPerson.ActorMover.Velocity=new Vector2(
                dir * xSpeed,
                yScale.Value * aniY.curve.Evaluate(timer*playSpeed));
            timer += Time.fixedDeltaTime;
        }

        public override TaskStatus OnUpdate()
        {
            if (timer < expectedTime.Value)
            {
//                Debug.Log($"Running: Timer:{timer},expectedTime:{expectedTime.Value}");
                return TaskStatus.Running;
            }

            selfPerson.ActorMover.Velocity=Vector2.zero;
//            Debug.Log($"Success: Timer:{timer},expectedTime:{expectedTime.Value}");
            return TaskStatus.Success;
        }
    }
}