using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using HighFive.Global;
using HighFive.Model.Person;
using ReadyGamerOne.Rougelike.Person;
using UnityEngine;

namespace HighFive.AI.Actions
{
	public class Seek : Action {
		
		public string walkAni;
		public SharedFloat attackDistance;
		private IHighFiveCharacter player;
		private Actor actor;
		private Animator _animator;
		private IHighFivePerson self;
		
		public override void OnStart()
		{
			self = gameObject.GetPersonInfo() as IHighFivePerson;
			
			player=GlobalVar.G_Player as IHighFiveCharacter;
			
			actor = gameObject.GetComponent<Actor>();
			_animator = gameObject.GetComponent<Animator>();
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
			
			if (Vector2.Distance(player.position, transform.position) < attackDistance.Value)
			{
//				Debug.Log("距离在攻击范围内，转到Attack");
				return TaskStatus.Success;
			}

			self.LookAt(player.transform);

			actor.PatrolOneDirInDistance(Mathf.Abs(player.position.x - transform.position.x),
				player.position.x > transform.position.x);
			
			return TaskStatus.Running;
		}
    }

}

