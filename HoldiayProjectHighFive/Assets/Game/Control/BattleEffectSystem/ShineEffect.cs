using Game.Control.PersonSystem;
using Game.Script;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Game.Control.BattleEffectSystem
{
    /// <summary>
    /// 闪烁效果
    /// </summary>
    public class ShineEffect : AbstractBattleEffect
    {
        private SpriteRenderer sr;
        private bool flag;
        private float lastTime;
        private float durTime;
        private Color color;
        private AbstractPerson ap;

        public ShineEffect(float lastTime, float durTime, Color color)
        {
            this.flag = false;
            this.lastTime = lastTime;
            this.durTime = durTime;
            this.color = color;
        }
        public override void Execute(AbstractPerson ap)
        {
            this.ap = ap;
            sr = ap.obj.GetComponent<SpriteRenderer>();
            int x = (int)(this.lastTime / this.durTime);
            int n = x % 2 == 0 ? x + 1 : x;

            MainLoop.Instance.ExecuteEverySeconds(_Execute, n, this.durTime, _Reset);
        }

        private void _Execute()
        {
            if (this.sr == null)
                return;

            if (this.flag)
                this.sr.color = Color.white;
            else
                this.sr.color = this.color;
            this.flag = !this.flag;
        }

        private void _Reset()
        {
            this.Release(this.ap);
            if (sr == null)
                return;
            sr.color = Color.white;
        }
    }
}
