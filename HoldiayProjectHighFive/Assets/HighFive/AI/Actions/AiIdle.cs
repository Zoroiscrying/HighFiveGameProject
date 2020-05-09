using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using UnityEngine;

namespace HighFive.AI.Actions
{
    public class AiIdle:Action
    {
        public SharedString idleAniName;
        public RangeInt idleTime;
        private Animator _animator;
        private float time;
        private float timer;
        public override void OnStart()
        {
            time = Random.Range(idleTime.start, idleTime.end);
            timer = 0;
            
            if(!string.IsNullOrEmpty(idleAniName.Value))
                GetComponent<Animator>()?.Play(Animator.StringToHash(idleAniName.Value));
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