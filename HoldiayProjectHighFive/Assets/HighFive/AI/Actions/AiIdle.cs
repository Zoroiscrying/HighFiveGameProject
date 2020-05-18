using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using UnityEngine;

namespace HighFive.AI.Actions
{
    public class AiIdle:Action
    {
        [BehaviorDesigner.Runtime.Tasks.Tooltip("Idle动画名")]
        public SharedString idleAniName;

        
        [BehaviorDesigner.Runtime.Tasks.Tooltip("固定等待时间")]
        public SharedFloat waitTime = 0;
        
        [BehaviorDesigner.Runtime.Tasks.Tooltip("是否随机等待")]
        public bool isRandom = true;
        [BehaviorDesigner.Runtime.Tasks.Tooltip("随机时间范围")]
        public RangeInt idleTime;


        private Animator _animator;
        private float time;
        private float timer;
        public override void OnAwake()
        {
            base.OnAwake();
            _animator = GetComponent<Animator>();
        }

        public override void OnStart()
        {
            time = isRandom ? Random.Range(idleTime.start, idleTime.end) : waitTime.Value;
            timer = 0;
            
            if(!string.IsNullOrEmpty(idleAniName.Value))
                _animator?.Play(Animator.StringToHash(idleAniName.Value));
        }

        public override TaskStatus OnUpdate()
        {
            timer += Time.deltaTime;
            if (timer > time)
                return TaskStatus.Success;
            return TaskStatus.Running;
            
        }
    }
}