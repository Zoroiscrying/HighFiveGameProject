using Game.Const;
using Game.Control.PersonSystem;
using ReadyGamerOne.Script;

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
