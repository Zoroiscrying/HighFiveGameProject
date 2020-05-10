using System;
using BehaviorDesigner.Runtime.Tasks;
using HighFive.Control.SkillSystem;
using HighFive.Model.Person;
using ReadyGamerOne.Rougelike.Person;
using UnityEngine;
using Action = BehaviorDesigner.Runtime.Tasks.Action;

namespace HighFive.AI.Actions
{
	[TaskDescription("释放指定技能")]
	public class RunSkill : Action
	{
		public SkillInfoAsset SkillInfoAsset;
		
		private IHighFivePerson self;
		private float timer = 0;

		public override void OnStart()
		{
			if (null == SkillInfoAsset)
			{
				throw new Exception("SkillAsset is null ??");
			}
			self = gameObject.GetPersonInfo() as IHighFivePerson;
			self.RunSkill(SkillInfoAsset);
			timer = 0;
		}

		public override TaskStatus OnUpdate()
		{
			if (null == SkillInfoAsset)
			{
				throw new Exception("SkillAsset is null ??");
			}
			
			timer += Time.deltaTime;
			if (timer > SkillInfoAsset.LastTime)
				return TaskStatus.Success;

			return TaskStatus.Running;
		}
		
	}


}

