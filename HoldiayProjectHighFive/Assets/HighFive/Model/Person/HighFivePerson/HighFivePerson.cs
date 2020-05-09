using System;
using HighFive.Control.EffectSystem;
using HighFive.Control.Movers.Interfaces;
using HighFive.Control.SkillSystem;
using HighFive.Data;
using HighFive.View;
using ReadyGamerOne.Data;
using ReadyGamerOne.Rougelike.Mover;
using ReadyGamerOne.Rougelike.Person;
using ReadyGamerOne.Script;
using UnityEngine;
using UnityEngine.Assertions;
using Random = UnityEngine.Random;

namespace HighFive.Model.Person
{
	public interface IHighFivePerson:
		IPoolDataPerson,
		IEffector<AbstractPerson>,
		IRichDamage
	{
		int ChangeHp(int change);
		
		int Dir { get; set; }
		
		float DefaultConstTime { get; set; }

		IActorBaseControl ActorMover { get; }

		void RunSkill(SkillInfoAsset skillInfoAsset,params object[] args);
		void LookAt(Transform target);
	}

	public abstract class HighFivePerson<T>:
		PoolDataPerson<T>,
		IHighFivePerson
		where T : HighFivePerson<T>,new()
	{

		#region 血量

		public int ChangeHp(int change)
		{
			this.Hp =
				Mathf.Clamp(this.Hp + change, 0, this.MaxHp);
			return 0;
		}		

		#endregion

		private IActorBaseControl _actorBaseControl;

		public override void OnInstanciateObject()
		{
			base.OnInstanciateObject();
			_actorBaseControl = gameObject.GetComponent<IActorBaseControl>();
			Assert.IsNotNull(_actorBaseControl);
		}

		#region IRichDamage

		public float AttackSpeed { get; set; } = 1;
		public float AttackAdder { get; set; } = 0;
		public float AttackScale { get; set; } = 1;
		public float TakeDamageScale { get; set; } = 1;
		public float TakeDamageBlock { get; set; } = 0;
		public float DodgeRate { get; set; } = 0;
		public float CritRate { get; set; } = 0;
		public float CritScale { get; set; } = 2;
		public bool IsInvincible { get; set; } = false;
		public Vector2 Repulse { get; set; }=Vector2.zero;
		public float RepulseScale { get; set; } = 1;
		public bool IgnoreRepulse { get; set; } = false;

		#endregion

		#region ITakeDamageablePerson<T>


		public override float OnTakeDamage(AbstractPerson takeDamageFrom, float damage)
		{
			if (System.Math.Abs(damage) < 0.01f)
				Debug.LogWarning("伤害是 0 ？？");

			#region 根据IRichDamage计算最终伤害

			//如果无敌直接返回
			if (this.IsInvincible)
				return -1;

			//增伤或减伤
			var finalDamage = damage * this.TakeDamageScale;
			
			//闪避
			if (Random.Range(0, 1f) > this.DodgeRate)
			{// 闪避失败
				
				//计算格挡
				finalDamage -= this.TakeDamageBlock;

				if (finalDamage < 0)
					finalDamage = 0;

				//播放受击效果
				PlayAcceptEffects(takeDamageFrom as IHighFivePerson);
				
			}
			else
			{// 闪避成功
				finalDamage = 0;
			}
			
			#endregion
			
			
			var dir = (takeDamageFrom as IHighFivePerson).Dir;
			var realDamage = Mathf.RoundToInt(finalDamage);
			new DamageNumberUI(realDamage, 0, 30, Color.red, transform, dir);
			return base.OnTakeDamage(takeDamageFrom,realDamage);
		}

		public override float OnCauseDamage(AbstractPerson causeDamageTo, float damage)
		{
			var realDamage = base.OnCauseDamage(causeDamageTo, damage);
            if(realDamage >0)
				PlayAttackEffects(AttackEffects);
            return realDamage;
		}
		
		#endregion

		#region IEffector<AbstractPerson>

		public EffectInfoAsset AttackEffects => (this.Controller as HighFivePersonController).attackEffects;
		public EffectInfoAsset HitEffects => (this.Controller as HighFivePersonController).hitEffects;
		public EffectInfoAsset AcceptEffects => (this.Controller as HighFivePersonController).acceptEffects;
		public AbstractPerson EffectPlayer => this;		

		/// <summary>
		/// 播放受击特效
		/// </summary>
		/// <param name="ditascher"></param>
		public void PlayAcceptEffects(IEffector<AbstractPerson> ditascher)
		{
			if (ditascher != null && ditascher.HitEffects != null)
			{
				ditascher.HitEffects.Play(ditascher, this);
			}

			if (AcceptEffects != null)
			{
				AcceptEffects.Play(null,this);
			}
		}

		
		/// <summary>
		/// 播放攻击特效
		/// </summary>
		/// <param name="effectInfoAsset"></param>
		public void PlayAttackEffects(EffectInfoAsset effectInfoAsset)
		{
			effectInfoAsset?.Play(this,null);
		}



		#endregion

		#region IHighFivePerson

		public virtual int Dir
		{
			get
			{
				return (this.Controller as HighFivePersonController).Dir;
			}
			set { (this.Controller as HighFivePersonController).Dir = value; }
		}

		public IActorBaseControl ActorMover => _actorBaseControl;
		
		public override Vector3 position
		{
			get => ActorMover.Position;
			set => ActorMover.Position = value;
		}

		public float DefaultConstTime { get; set; }




		/// <summary>
		/// 执行技能
		/// </summary>
		/// <param name="skillInfoAsset"></param>
		///
		/// 
		public void RunSkill(SkillInfoAsset skillInfoAsset,params object[] args)
		{
			skillInfoAsset.RunSkill(this,args: args);
		}

		public void LookAt(Transform target)
		{
			(Controller as HighFivePersonController).LookAt(target);
		}		

		#endregion
	}
}
