using Game.Const;
using Game.Control.Person;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Game.Control.BattleEffects
{
    /// <summary>
    /// 即时扣血效果,包括伤害数字的现实
    /// </summary>
    public class InstantDamageEffect : AbstractBattleEffect
    {
        private int damage;
        private int dir;

        public InstantDamageEffect(int damage, int dir)
        {
            this.dir = dir;
            this.damage = damage;
        }

        public override void Execute(AbstractPerson ap)
        {

            //            Debug.Log(ap.obj.GetInstanceID()+": "+ap.IsConst);
            //如果不可选定（硬直）,告辞
            //数值
            //            Debug.Log( ap.name + "收到" + this.damage + "伤害");

            //   BloodChange


            if (!ap.IsConst)
                ap.TakeBattleEffect(new DamageNumberEffect(this.damage, 0.3f * ap.Scanler, Color.red, this.dir, 1f, UnityEngine.Random.Range(10, 60)));

            CEventCenter.BroadMessage(Message.M_BloodChange(ap.obj), -this.damage);
            this.Release(ap);
        }
    }
}
