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
    [TaskDescription("使用曲线控制Y向速度的方式，在指定时间内往指定方向跳跃指定距离")]
    public class AiCurvedJumpDistance:Action
    {
        [BehaviorDesigner.Runtime.Tasks.Tooltip("往那个方向跳")]
        public SharedInt inTargetDir;
        [BehaviorDesigner.Runtime.Tasks.Tooltip("跳多大距离")]
        public SharedFloat exceptedDistance;
        [BehaviorDesigner.Runtime.Tasks.Tooltip("几秒之内结束")]
        public SharedFloat expectedTime;
        [BehaviorDesigner.Runtime.Tasks.Tooltip("跳跃高度缩放")]
        public SharedFloat yScale = 1.0f;
        [BehaviorDesigner.Runtime.Tasks.Tooltip("跳跃速度曲线资源")]
        public AnimationCurveAsset aniY;
        
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
            var selfPosition = selfPerson.position;
            var targetPosition = selfPosition + inTargetDir.Value * exceptedDistance.Value * Vector3.right;

            var xDis = Mathf.Abs(targetPosition.x - selfPosition.x);
            
            xSpeed = xDis / expectedTime.Value;
            playSpeed = animationTime / expectedTime.Value;
            
            
            timer = 0;
            dir = targetPosition.x > selfPosition.x ? 1 : -1;
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
//                Debug.Log($"Timer:{timer},playSpeed:{playSpeed}");
                return TaskStatus.Running;
            }

            selfPerson.ActorMover.Velocity=Vector2.zero;
            return TaskStatus.Success;
        }
    }
}