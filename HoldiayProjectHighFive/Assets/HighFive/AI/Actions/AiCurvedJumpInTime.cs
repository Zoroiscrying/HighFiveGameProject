using System.Linq;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using ReadyGamerOne.Rougelike.Mover;
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

        private float playSpeed;
        private float xSpeed = 1.0f;
        private float animationTime;
        private IMover2D selfMover;

        public override void OnAwake()
        {
            base.OnAwake();
            selfMover = gameObject.GetComponent<IMover2D>();
            Assert.IsNotNull(selfMover);
            this.animationTime = aniY.curve.keys.Last().time;
        }

        private float timer = 0;
        private int dir = 0;
        public override void OnStart()
        {
            base.OnStart();

            var targetPosition = inTargetPosition.Value;
            var selfPosition = transform.position;
            var xDis = Mathf.Abs(targetPosition.x - selfPosition.x);
            
            xSpeed = xDis / expectedTime.Value;
            playSpeed = animationTime / expectedTime.Value;
            
            
            timer = 0;
            dir = targetPosition.x > selfPosition.x ? 1 : -1;
        }

        public override void OnFixedUpdate()
        {
            base.OnFixedUpdate();
            selfMover.Velocity=new Vector2(
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

            selfMover.Velocity=Vector2.zero;
//            Debug.Log($"Success: Timer:{timer},expectedTime:{expectedTime.Value}");
            return TaskStatus.Success;
        }
    }
}