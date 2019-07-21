using Game.Control.PersonSystem;
using Game.Script;
using System;
using Game.Const;
using Game.Data;
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

        public override void LoadTxt(string args)//type, int id,float startTime, float lastTime, string args = "")
        {
            var strs = args.Split(TxtManager.SplitChar);
            Assert.IsTrue(strs.Length >= BasePropertyCount+3);
            this.StartSpeed = Convert.ToSingle(strs[BasePropertyCount].Trim());
            this.EndSpeed = Convert.ToSingle(strs[BasePropertyCount+1].Trim());
            this.dur = Convert.ToSingle(strs[BasePropertyCount+2].Trim());
            if (dur <= 0 || dur >= 1)
                Debug.LogError("差值不合理 " + dur);
            base.LoadTxt(string.Join(TxtManager.SplitChar.ToString(), strs, 0, this.BasePropertyCount));
        }
        
        public override string Sign
        {
            get
            {
                return TxtSign.dash;
            }
        }
    }
}
