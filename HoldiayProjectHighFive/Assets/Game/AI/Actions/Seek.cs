using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using Game.Control.PersonSystem;
using ReadyGamerOne.Global;
using UnityEngine;

namespace Game.AI
{
	public class Seek : Action {
		
		public string walkAni;
		public SharedFloat attackDistance;
		private AbstractPerson player;
		private Actor actor;
		private Animator _animator;
		private AbstractPerson self;
		
		public override void OnStart()
		{
			player = GlobalVar.G_Player;
			actor = gameObject.GetComponent<Actor>();
			_animator = gameObject.GetComponent<Animator>();
			self = AbstractPerson.GetInstance(gameObject);
			_animator.Play(Animator.StringToHash(walkAni));
		}

		public override void OnDrawGizmos()
		{
			var color = Gizmos.color;
			Gizmos.color=Color.red;
			Gizmos.DrawWireSphere(transform.position, attackDistance.Value);
			Gizmos.color = color;
		}

		public override TaskStatus OnUpdate()
		{
			
			if (Vector2.Distance(player.Pos, transform.position) < attackDistance.Value)
			{
//				Debug.Log("距离在攻击范围内，转到Attack");
				return TaskStatus.Success;
			}

			self.LookAt(player.obj.transform);

			actor.PatrolOneDirInDistance(Mathf.Abs(player.Pos.x - transform.position.x),
				player.Pos.x > transform.position.x);
			
			return TaskStatus.Running;
		}
    }

}

