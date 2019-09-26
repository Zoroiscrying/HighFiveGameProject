using BehaviorDesigner.Runtime.Tasks;
using Game.Control.PersonSystem;
using Game.Control.SkillSystem;
using UnityEngine;

namespace Game.AI
{
	public class Attack : Action
	{

		public SkillInfoAsset SkillInfoAsset;
		private AbstractPerson self;

		private Animator _animator;
		private float timer = 0;

		public override void OnStart()
		{
			self = AbstractPerson.GetInstance(gameObject);
			Debug.Log("进行技能："+SkillInfoAsset.skillName.StringValue);
			self.RunSkill(SkillInfoAsset);
			timer = 0;
			gameObject.GetComponent<Actor>()._velocity = Vector3.zero;
			
		}

		public override TaskStatus OnUpdate()
		{
			timer += Time.deltaTime;
			if (timer > SkillInfoAsset.LastTime)
				return TaskStatus.Success;

			return TaskStatus.Running;
		}
		
	}


}

