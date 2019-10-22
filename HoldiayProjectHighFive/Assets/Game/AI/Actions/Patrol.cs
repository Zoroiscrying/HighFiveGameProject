using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using Game.Control.PersonSystem;
using UnityEngine;

namespace Game.AI
{
	public class Patrol : Action {
    
    	// Use this for initialization
//        public SharedFloat moveSpeed;
//        public SharedFloat attachDistance = 0.3f;
        public string walkAni;

        public SharedFloat maxPatrolTime;
//
//        public SharedTransform mark1;
//        public SharedTransform mark2;

        private Actor actor;
        private Vector3 dir1;
        private Vector3 dir2;
        private AbstractPerson self;
        private float timer = 0;
        public override void OnStart()
        {
	        base.OnStart();
	        actor = gameObject.GetComponent<Actor>();
	        self = AbstractPerson.GetInstance(gameObject);
	        gameObject.GetComponent<Animator>().Play(Animator.StringToHash(walkAni));
	        timer = 0;

        }

        public override TaskStatus OnUpdate()
        {
	        timer += Time.deltaTime;
	        if (timer > maxPatrolTime.Value)
		        return TaskStatus.Success;
	        self.Dir = actor._velocity.x > 0 ? 1 : -1;
	        actor.Patrol();
//	        Debug.Log("巡逻中，正在往" + self.Dir + " 方向走");
	        return TaskStatus.Running;
        }
	}

}

