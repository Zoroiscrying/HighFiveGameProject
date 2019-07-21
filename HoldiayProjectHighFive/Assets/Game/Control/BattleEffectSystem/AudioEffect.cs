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
//            Debug.Log("音乐："+audioName);
            AudioMgr.Instance.PlayEffect(this.audioName, ap.obj.transform.position);
            this.Release(ap);
        }
    }
}
