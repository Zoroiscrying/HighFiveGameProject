using Game.Control.PersonSystem;
using Game.Script;
using System;
using UnityEngine;
using UnityEngine.Assertions;

namespace Game.Control.SkillSystem
{
    /// <summary>
    /// 技能位移触发器
    /// </summary>
    public class DashTrigger : AbstractSkillTrigger
    {
        private MainCharacter controller;
        private float StartSpeed;
        private float EndSpeed;
        private float dur;


        public override void Execute(AbstractPerson self)
        {
            this.controller = self.obj.gameObject.GetComponent<MainCharacter>();
            MainLoop.Instance.ExecuteLater(_SetSpeed, this.StartTime, self);
            MainLoop.Instance.UpdateForSeconds(_Execute, this.LastTime, self, this.StartTime);
        }

        private void _SetSpeed(AbstractPerson self)
        {
            controller._playerVelocityX = self.Dir * this.StartSpeed;
        }

        private void _Execute(AbstractPerson self)
        {
            controller._playerVelocityX = self.Dir * Mathf.Lerp(Mathf.Abs(controller._playerVelocityX), this.EndSpeed, this.dur);
        }

        public override void Init(string args)//type, int id,float startTime, float lastTime, string args = "")
        {
            var strs = args.Split('|');
            Assert.IsTrue(strs.Length >= 7);
            this.StartSpeed = Convert.ToSingle(strs[4].Trim());
            this.EndSpeed = Convert.ToSingle(strs[5].Trim());
            this.dur = Convert.ToSingle(strs[6].Trim());
            if (dur <= 0 || dur >= 1)
                Debug.LogError("差值不合理 " + dur);
            base.Init(string.Join("|", strs, 0, this.BasePropertyCount));
        }
    }
}
