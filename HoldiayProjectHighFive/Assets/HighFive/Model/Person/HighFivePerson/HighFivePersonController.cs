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
		public IHighFivePerson HighFivePerson=>selfPerson as IHighFivePerson;
		
		
		/// <summary>
		/// 看向某个物体，【当然，只是左右】
		/// </summary>
		/// <param name="target"></param>
		private SpriteRenderer sr;
		public void LookAt(Transform target)
		{
			HighFivePerson.Dir = target.position.x > selfPerson.position.x ? 1 : -1;
		}
		public EffectInfoAsset attackEffects;
		public EffectInfoAsset hitEffects;
		public EffectInfoAsset acceptEffects;
//		public CharacterController2D characterController;
//		public Actor actor;
		public virtual int Dir
		{
			get => HighFivePerson.ActorMover.FaceDir;
			set => HighFivePerson.ActorMover.FaceDir = value;
		}

		public override void SetMoveable(bool state)
		{
			HighFivePerson.ActorMover.SetMovable(state);
		}
	}
}
