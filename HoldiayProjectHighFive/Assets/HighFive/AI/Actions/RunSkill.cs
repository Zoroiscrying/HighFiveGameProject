using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using HighFive.Control.SkillSystem;
using HighFive.Global;
using HighFive.Model.Person;
using ReadyGamerOne.Rougelike.Person;
using UnityEngine;
using UnityEngine.Assertions;

namespace HighFive.AI.Actions
{
	public class RunSkill : Action
	{
		public SkillInfoAsset SkillInfoAsset;
		
		private IHighFivePerson self;
		private float timer = 0;

		public override void OnStart()
		{
			if (null == SkillInfoAsset)
				return;
			self = gameObject.GetPersonInfo() as IHighFivePerson;
			self.RunSkill(SkillInfoAsset);
			timer = 0;
			var actor=gameObject.GetComponent<Actor>();
			if(actor)
				actor._velocity = Vector3.zero;
		}

		public override TaskStatus OnUpdate()
		{
			if (null == SkillInfoAsset)
				return TaskStatus.Failure;
			
			timer += Time.deltaTime;
			if (timer > SkillInfoAsset.LastTime)
				return TaskStatus.Success;

			return TaskStatus.Running;
		}
		
	}


}

