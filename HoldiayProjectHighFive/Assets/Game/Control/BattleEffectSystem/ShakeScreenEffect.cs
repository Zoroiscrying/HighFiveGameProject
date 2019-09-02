using Game.Control.PersonSystem;
using Game.Scripts;
using UnityEngine;

namespace Game.Control.BattleEffectSystem
{
    /// <summary>
    /// 震屏效果
    /// </summary>
    public class ShakeScreenEffect : AbstractBattleEffect
    {
        private float howmuch;
        private float time;

        public ShakeScreenEffect(float howmuch, float time)
        {
            this.howmuch = howmuch;
            this.time = time;
        }

        public override void Execute(AbstractPerson ap)
        {
            ScreenShake.Instance.enabled = false;
            ScreenShake.Instance.enabled = true;

            ScreenShake.Instance.shakeDir = this.howmuch * Vector3.one;
            ScreenShake.Instance.shakeTime = this.time;
            this.Release(ap);
        }
    }
}
