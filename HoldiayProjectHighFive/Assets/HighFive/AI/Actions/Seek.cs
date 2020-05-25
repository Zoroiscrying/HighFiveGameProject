using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using HighFive.Global;
using HighFive.Model.Person;
using ReadyGamerOne.Rougelike.Person;
using UnityEngine;
using Action = BehaviorDesigner.Runtime.Tasks.Action;

namespace HighFive.AI.Actions
{
	public class Seek : Action {
		
		public string walkAni;
		public SharedFloat attackDistance;
		private Animator _animator;
		private IHighFivePerson self;
		
		public override void OnStart()
		{
			self = gameObject.GetPersonInfo() as IHighFivePerson;
			
			_animator = gameObject.GetComponent<Animator>();
			_animator.Play(Animator.StringToHash(walkAni));
		}

		public override void OnDrawGizmos()
		{
			if (null == transform)
				return;
			var color = Gizmos.color;
			Gizmos.color=Color.red;
			Gizmos.DrawWireSphere(transform.position, attackDistance.Value);
			Gizmos.color = color;
		}

		public override TaskStatus OnUpdate()
		{
			
			if (Vector2.Distance(GlobalVar.G_Player.position, transform.position) < attackDistance.Value)
			{
//				Debug.Log("距离在攻击范围内，转到Attack");
				return TaskStatus.Success;
			}

			self.LookAt(GlobalVar.G_Player.transform);
			
			return TaskStatus.Running;
		}

		private float PatrolDistance
		{
			get
			{
				var ans = Mathf.Abs(GlobalVar.G_Player.position.x - transform.position.x)-attackDistance.Value;
				ans = Mathf.Max(0, ans);
				return ans;

			}
		}
    }

}

