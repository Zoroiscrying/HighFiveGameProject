using Game.Control.BattleEffectSystem;
using System.Collections.Generic;
using Game.Const;
using Game.View;
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

        public TestPerson(BaseCharacterInfo characterInfo,Vector3 pos, Transform parent = null) : base(characterInfo,pos, parent)
        {
            new BloodBarUI(this);
        }

        #region BattleEffect
        public override void TakeBattleEffect(List<IBattleEffect> beList)
        {
            base.TakeBattleEffect(beList);
            new AudioEffect(AudioName._Fuck).Execute(this);
            new ShineEffect(this.shineLastTime, this.shineDurTime, Color.red).Execute(this);
            new ShakeScreenEffect(0.02f, Time.deltaTime).Execute(this);
        }

        protected override void AddBaseAttackEffects(AbstractPerson self)
        {
            TestPerson tp = self as TestPerson;
            Assert.IsTrue(tp != null);
            tp.attackEffects.Add(new InstantDamageEffect(this.BaseAttack, this.Dir));
            tp.attackEffects.Add(new HitbackEffect(new Vector2(this.hitback, 0)));
        }
        #endregion
    }
}
