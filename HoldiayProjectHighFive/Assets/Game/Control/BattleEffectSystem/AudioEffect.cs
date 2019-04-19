using Game.Control.PersonSystem;
using Game.Script;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Game.Control.BattleEffectSystem
{
    /// <summary>
    /// 音效
    /// </summary>
    public class AudioEffect : AbstractBattleEffect
    {
        private string audioName;

        public AudioEffect(string audioName)
        {
            this.audioName = audioName;
        }

        public override void Execute(AbstractPerson ap)
        {
            AudioMgr.Instance.PlayAuEffect(this.audioName, ap.obj.transform.position);
            this.Release(ap);
        }
    }
}
