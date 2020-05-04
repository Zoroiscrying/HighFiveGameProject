using HighFive.Control.EffectSystem;
using ReadyGamerOne.Rougelike.Person;
using UnityEngine;
using UnityEngine.Assertions;
using zoroiscrying;

namespace HighFive.Model.Person
{
	/// <summary>
	/// Person角色控制类，在角色类加UsePersonController属性的话会自动添加上去，使用InitController初始化
	/// </summary>
	public abstract class HighFivePersonController : AbstractPersonController
	{
		/// <summary>
		/// 看向某个物体，【当然，只是左右】
		/// </summary>
		/// <param name="target"></param>
		private SpriteRenderer sr;
		public void LookAt(Transform target)
		{
			if (!sr)
			{
				sr = GetComponent<SpriteRenderer>();
				Assert.IsTrue(sr);
			}

			var dir = target.transform.position.x - selfPerson.position.x > 0;
			Dir = dir ? 1 : -1;
		}
		public EffectInfoAsset attackEffects;
		public EffectInfoAsset hitEffects;
		public EffectInfoAsset acceptEffects;
		public CharacterController2D characterController;
		public Actor actor;
		public virtual int Dir
		{
			get { return actor._faceDir; }
			set { actor._faceDir = value; }
		}

		public override void SetMoveable(bool state)
		{
			if(actor)
				actor.enabled = state;
			if(characterController)
				characterController.enabled = state;
		}
	}
}
