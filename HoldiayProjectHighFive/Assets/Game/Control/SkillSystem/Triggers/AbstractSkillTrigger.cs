using Game.Control.Person;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine.Assertions;

namespace Game.Control.SkillSystem
{
    /// <summary>
    /// 技能触发器抽象类
    /// </summary>
    public abstract class AbstractSkillTrigger : ISkillTrigger
    {
        public float LastTime { get; set; }
        public int id { get; set; }
        public float StartTime { get; set; }
        public string SkillType { get; set; }
        public bool IsExecuted { get; set; }

        public virtual int BasePropertyCount
        {
            get
            {
                return 4;
            }
        }

        public virtual void Release()
        {
            this.IsExecuted = false;
        }


        /// <summary>
        /// 这里要完成startTime,lastTime，SkillType,IsExecuted以及其他额外参数的初始化
        /// </summary>
        /// <param name="type"></param>
        /// <param name="args"></param>
        public virtual void Init(string args)//string type, int id,float startTime,float lastTime,string args="")
        {
            var strs = args.Split('|');
            Assert.IsTrue(strs.Length >= 4);
            this.IsExecuted = false;
            this.LastTime = Convert.ToSingle(strs[3].Trim());
            this.StartTime = Convert.ToSingle(strs[1].Trim());
            this.id = Convert.ToInt32(strs[2].Trim());
            this.SkillType = strs[0].Trim();
        }

        /// <summary>
        /// 触发函数
        /// </summary>
        /// <param name="self"></param>
        /// <returns></returns>
        public abstract void Execute(AbstractPerson self);
    }


}
