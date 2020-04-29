using HighFive.Control.EffectSystem;
using HighFive.Control.SkillSystem;
using ReadyGamerOne.Rougelike.Person;
using ReadyGamerOne.Script;
using UnityEngine;
using UnityEngine.Assertions;

namespace HighFive.Model.Person
{
	public interface IHighFivePerson:
		IPoolDataPerson,
		IEffector<AbstractPerson>
	{
		int ChangeHp(int change);
		
		int Dir { get; set; }
		bool IsConst { get; set; }
		bool IgnoreHitback { get; set; }
		float DefaultConstTime { get; set; }
		float AttackSpeed { get; set; }
		Vector2 HitBackSpeed { get; }

		void RunSkill(SkillInfoAsset skillInfoAsset);
		void LookAt(Transform target);
	}

	public abstract class HighFivePerson<T>:
		PoolDataPerson<T>,
		IHighFivePerson
		where T : HighFivePerson<T>,new()
	{
		#region Fields
		
		protected Vector2 _hitBackSpeed;

		#endregion

		#region 血量

		public int ChangeHp(int change)
		{
			this.Hp =
				Mathf.Clamp(this.Hp + change, 0, this.MaxHp);
			return 0;
		}		

		#endregion

		public virtual int Dir
		{
			get
			{
				return (this.Controller as HighFivePersonController).Dir;
			}
			set { (this.Controller as HighFivePersonController).Dir = value; }
		} 

		public bool IsConst { get; set; }
		public bool IgnoreHitback { get; set; }

		public float DefaultConstTime { get; set; }

		public float AttackSpeed { get; set; } = 1;

		public Vector2 HitBackSpeed
		{
			get { return _hitBackSpeed; }
		}


		#region ITakeDamageablePerson<T>


		public override void OnTakeDamage(AbstractPerson takeDamageFrom, int damage)
		{
			if (damage == 0)
				Debug.LogWarning("伤害是 0 ？？");
			
			//播放受击动画
			PlayAcceptEffects(takeDamageFrom as IHighFivePerson);
			
			base.OnTakeDamage(takeDamageFrom,damage);
		}

		public override void OnCauseDamage(AbstractPerson causeDamageTo, int damage)
		{
			base.OnCauseDamage(causeDamageTo, damage);
			PlayAttackEffects(AttackEffects);
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

		/// <summary>
		/// 执行技能
		/// </summary>
		/// <param name="skillInfoAsset"></param>
		///
		/// 
		public void RunSkill(SkillInfoAsset skillInfoAsset)
		{
			skillInfoAsset.RunSkill(this);
		}

		public void LookAt(Transform target)
		{
			(Controller as HighFivePersonController).LookAt(target);
		}
		
		/// <summary>
		/// 变为硬直状态
		/// </summary>
		public void ToConst(float time)
		{
			Assert.IsTrue(!IsConst);
			IsConst = !IsConst;
			//            Debug.Log(this.obj.GetInstanceID() + "硬直 "+this.IsConst);
			MainLoop.Instance.ExecuteLater(_Reset, time);
			//硬直动画
		}
		private void _Reset()
		{
			//            Debug.Log(this.obj.GetInstanceID() + "恢复");
			Assert.IsTrue(IsConst);
			IsConst = !IsConst;
		}
	}
}
