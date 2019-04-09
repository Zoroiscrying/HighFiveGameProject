using Game.Control.Person;
using Game.Script;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine.Assertions;

namespace Game.Control.SkillSystem
{
    /// <summary>
    /// 音效触发器
    /// </summary>
    public class AudioTrigger : AbstractSkillTrigger
    {
        private string audioName;

        public override void Init(string args)//type, int id,float startTime, float lastTime, string args = "")
        {
            var strs = args.Split('|');
            Assert.IsTrue(strs.Length >= 5);
            this.audioName = strs[4].Trim();
            base.Init(string.Join("|", strs, 0, 4));
        }

        public override void Execute(AbstractPerson self)
        {
            MainLoop.Instance.ExecuteLater(_Execute, this.StartTime, self);
        }

        private void _Execute(AbstractPerson ap)
        {
            AudioMgr.Instance.PlayAuEffect(this.audioName, ap.obj.transform.position);

        }
    }
}
