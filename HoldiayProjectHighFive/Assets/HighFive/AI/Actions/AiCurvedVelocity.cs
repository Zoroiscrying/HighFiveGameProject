using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using ReadyGamerOne.Rougelike.Mover;
using ReadyGamerOne.ScriptableObjects;
using UnityEngine;
using UnityEngine.Assertions;

namespace HighFive.AI.Actions
{
    public class AiCurvedVelocity:Action
    {
        public SharedGameObject inTarget;
        
        public SharedFloat time;
        public SharedFloat xScale = 1.0f;
        public SharedFloat yScale = 1.0f;
        public SharedAnimationCurve aniX;
        public AnimationCurveAsset aniY;

        private IMover2D selfMover;

        public override void OnAwake()
        {
            base.OnAwake();
            selfMover = gameObject.GetComponent<IMover2D>();
            Assert.IsNotNull(selfMover);
        }

        private float timer = 0;
        private int dir = 0;
        public override void OnStart()
        {
            base.OnStart();
            timer = 0;
            dir = inTarget.Value.transform.position.x > transform.position.x ? 1 : -1;
        }

        public override void OnFixedUpdate()
        {
            base.OnFixedUpdate();
            selfMover.Velocity=new Vector2(
                dir * xScale.Value * aniX.Value.Evaluate(timer),
                yScale.Value * aniY.curve.Evaluate(timer));
            timer += Time.fixedDeltaTime;
        }

        public override TaskStatus OnUpdate()
        {
            if (timer < time.Value)
            {
                return TaskStatus.Running;
            }

            selfMover.Velocity=Vector2.zero;
            return TaskStatus.Success;
        }
    }
}