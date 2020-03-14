using BehaviorDesigner.Runtime.Tasks;
using HighFive.Control.SkillSystem;
using HighFive.Global;
using HighFive.Model.Person;
using ReadyGamerOne.Rougelike.Person;
using UnityEngine;

namespace HighFive.AI.Actions
{
	public class RunSkill : Action
	{
		public SkillInfoAsset SkillInfoAsset;
		private IHighFivePerson self;

		private Animator _animator;
		private float timer = 0;

		public override void OnStart()
		{
			self = gameObject.GetPersonInfo() as IHighFivePerson;
			SkillInfoAsset.Vector3Cache = GlobalVar.G_Player.position;
//			Debug.Log($"写入用户数据：【{SkillInfoAsset.skillName.StringValue}】" + SkillInfoAsset.Vector3Cache);
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

