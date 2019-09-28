using BehaviorDesigner.Runtime.Tasks;
using Game.Control.PersonSystem;
using Game.Control.SkillSystem;
using ReadyGamerOne.Global;
using UnityEngine;

namespace Game.AI
{
	public class RunSkill : Action
	{
		public SkillInfoAsset SkillInfoAsset;
		private AbstractPerson self;

		private Animator _animator;
		private float timer = 0;

		public override void OnStart()
		{
			self = AbstractPerson.GetInstance(gameObject);
			SkillInfoAsset.Vector3Cache = GlobalVar.G_Player.Pos;
			Debug.Log($"写入用户数据：【{SkillInfoAsset.skillName.StringValue}】" + SkillInfoAsset.Vector3Cache);
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

