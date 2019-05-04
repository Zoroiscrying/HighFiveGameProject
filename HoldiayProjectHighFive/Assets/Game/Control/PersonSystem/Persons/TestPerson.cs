using Game.Control.BattleEffectSystem;
using Game.View;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.Assertions;

namespace Game.Control.PersonSystem
{
    /// <summary>
    /// 测试草人
    /// </summary>
    public class TestPerson : AbstractPerson
    {
        #region Private

        private float shineLastTime = 1;
        private float shineDurTime = 0.1f;
        private float hitback = 0.05f;

        #endregion

        public TestPerson(string name, string prefabPath, Vector3 pos, List<string> skillTypes=null, Transform parent = null) : base(name, prefabPath, pos, skillTypes, parent)
        {
            this.MaxHp = 100000000;
            this.Hp = 2000000000;
        }

        #region BattleEffect
        public override void TakeBattleEffect(List<IBattleEffect> beList)
        {
            base.TakeBattleEffect(beList);
            new AudioEffect("Fuck").Execute(this);
            new ShineEffect(this.shineLastTime, this.shineDurTime, Color.red).Execute(this);
            new ShakeScreenEffect(0.02f, Time.deltaTime).Execute(this);
        }

        protected override void AddBaseAttackEffects(AbstractPerson self)
        {
            TestPerson tp = self as TestPerson;
            Assert.IsTrue(tp != null);
            tp.attackEffects.Add(new InstantDamageEffect(this.Attack, this.Dir));
            tp.attackEffects.Add(new HitbackEffect(new Vector2(this.hitback, 0)));
        }
        #endregion
    }
}
