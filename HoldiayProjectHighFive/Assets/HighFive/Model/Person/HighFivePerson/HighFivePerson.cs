using System;
using DefaultNamespace.HighFive.Model.Person;
using HighFive.Control.EffectSystem;
using HighFive.Control.Movers.Interfaces;
using HighFive.Control.SkillSystem;
using HighFive.Data;
using HighFive.View;
using ReadyGamerOne.Data;
using ReadyGamerOne.Rougelike.Person;
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

		private string _personName="未设置名字";
		public override string CharacterName
		{
			get => _personName;
			set => _personName = value;
		}

		public override void LoadData(CsvMgr data)
		{
			base.LoadData(data);
			var personData = CsvData as PersonData;
			this.Repulse = new Vector2(personData.hitback_x, personData.hitback_y);
			this.CharacterName = personData.personName;
		}

		#region IPoolablePerson

		public override void OnGetFromPool()
		{
			base.OnGetFromPool();
			_actorBaseControl = gameObject.GetComponent<IActorBaseControl>();
			if (null == _actorBaseControl)
			{
				throw new Exception($"{CharacterName}的actorBaseControl is null");
			}else 
				Debug.Log($"{CharacterName}加载_actorBaseControl");
		}

		public override void OnRecycleToPool()
		{
			base.OnRecycleToPool();
			_actorBaseControl = null;
		}
		

		#endregion
		
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

		public override BasicDamage CalculateDamage(float skillDamageScale, AbstractPerson receiver)
		{
			return new HighFiveDamage(this, receiver as IHighFivePerson, skillDamageScale);
		}

		public override float OnTakeDamage(AbstractPerson takeDamageFrom, BasicDamage damage)
		{

			var richDamage = damage as HighFiveDamage;
			Assert.IsNotNull(richDamage);

			if (richDamage.IsInvincible)
				return -1;
			
			if (!richDamage.IsMissing)
			{
				//播放受击效果
				PlayAcceptEffects(takeDamageFrom as IHighFivePerson);
			}

			var attackPerson = takeDamageFrom as IHighFivePerson;
			
			var dir = takeDamageFrom.position.x > position.x ? 1 : -1;

			//伤害数字
			new DamageNumberUI(
				richDamage,
				0,
				transform, 
				dir);
			
			//击退
			var attackerRepulse = attackPerson.Repulse;
			attackerRepulse.x *= -dir;
			ActorMover.ChangeVelBasedOnHitDir(attackerRepulse,attackPerson.RepulseScale);
			
			return base.OnTakeDamage(takeDamageFrom,richDamage);
		}

		public override float OnCauseDamage(AbstractPerson causeDamageTo, BasicDamage damage)
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

		private IActorBaseControl _actorBaseControl;

		public IActorBaseControl ActorMover
		{
			get
			{
				if (null == _actorBaseControl)
				{
					throw new Exception($"{CharacterName}的actorBaseControl is null");
				}
				return _actorBaseControl;
			}
		}
		
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
