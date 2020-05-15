using HighFive.Const;
using HighFive.Global;
using HighFive.Model.Person;
using ReadyGamerOne.Common;
using ReadyGamerOne.Rougelike.Person;
using UnityEngine;
using UnityEngine.Assertions;

namespace DefaultNamespace.HighFive.Model.Person
{
    public class HighFiveDamage:BasicDamage
    {
        public bool IsCrit { get; private set; }
        public bool IsInvincible { get; private set; }
        public bool IsSuper { get; private set; }
        public bool IsBlock { get; private set; }
        public bool IsMissing { get; private set; }
        public bool IsSkill { get; private set; }
        public bool IsPlayer { get; private set; }

        public HighFiveDamage(IHighFivePerson person,IHighFivePerson receiver, float? skillDamageScale=null)
        {
            Assert.IsTrue(person!=null
                          && receiver!=null
                          && person.IsAlive
                          && receiver.IsAlive);
            
            
			if (System.Math.Abs(person.Attack) < 0.01f)
				Debug.LogWarning("伤害是 0 ？？");

            IsPlayer = person is IHighFiveCharacter;
            
           
            //如果无敌直接返回
            if (receiver.IsInvincible)
            {
                IsInvincible = true;
                Damage = 0;
                return;
            }
 
            
            this.OriginAttack = person.Attack;

            Damage = this.OriginAttack;
            if (Random.Range(0, 1f) < person.CritRate)
            {
                IsCrit = true;
                Damage *= person.CritScale;
            }
            //技能倍率
            if (null != skillDamageScale)
            {
                this.ScaleSkillDamageScale = skillDamageScale.Value;
                IsSkill = true;
            }
            else
                this.ScaleSkillDamageScale = 1.0f;
            Damage *= ScaleSkillDamageScale;
            
            //增伤或减伤
             Damage *= receiver.TakeDamageScale;
             
             //Super
             if (IsPlayer && GlobalVar.isSuper)
             {
                 IsSuper = true;
                 Damage *= GameSettings.Instance.superAttackScale;
                 CEventCenter.BroadMessage(Message.M_ExitSuper);
             }
			
            //闪避
            if (Random.Range(0, 1f) > receiver.DodgeRate)
            {// 闪避失败
				
                //计算格挡
                Damage -= receiver.TakeDamageBlock;

                IsBlock = true;

                if (Damage < 0)
                    Damage = 0;

            }
            else
            {// 闪避成功
                IsMissing = true;
                Damage = 0;
            }
        }
    }
}