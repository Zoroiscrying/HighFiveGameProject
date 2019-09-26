using BehaviorDesigner.Runtime.Tasks;
using UnityEngine;

namespace Game.AI
{
    public class IdleState:Action
    {
        public string idleAniName;
        private Animator _animator;
        public RangeInt idleTime;
        private float time;
        private float timer;
        public override void OnStart()
        {
            time = Random.Range(idleTime.start, idleTime.end);
            timer = 0;
            _animator.Play(Animator.StringToHash(idleAniName));
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