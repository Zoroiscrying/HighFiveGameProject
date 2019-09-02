using Game.Control.PersonSystem;
using System;
using System.IO;
using ReadyGamerOne.Data;
using UnityEngine.Assertions;

namespace Game.Control.SkillSystem
{
    /// <summary>
    /// 技能触发器抽象类
    /// </summary>
    public abstract class AbstractSkillTrigger : ISkillTrigger,ITxtSerializable
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
                return 3;
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
        public virtual void LoadTxt(string args)//string type, int id,float startTime,float lastTime,string args="")
        {
            var strs = args.Split(TxtManager.SplitChar);
            Assert.IsTrue(strs.Length >= this.BasePropertyCount);
            this.IsExecuted = false;
            this.LastTime = Convert.ToSingle(strs[2].Trim());
            this.StartTime = Convert.ToSingle(strs[0].Trim());
            this.id = Convert.ToInt32(strs[1].Trim());
        }

        /// <summary>
        /// 触发函数
        /// </summary>
        /// <param name="self"></param>
        /// <returns></returns>
        public abstract void Execute(AbstractPerson self);


#region ITxtSerializable
        public abstract string Sign { get; }

        public void SignInit(string initLine)
        {
            this.SkillType = initLine.Trim();
//            Debug.Log(this.SkillType + " SignInit");
        }

        public void LoadTxt(StreamReader sr)
        {
//            Debug.Log(this.SkillType + " Main");
            var line = TxtManager.ReadUntilValue(sr);
            Assert.IsFalse(string.IsNullOrEmpty(line));
            LoadTxt(line);
//            Debug.Log(this.SkillType+" end Main");
        }
    }
#endregion

}
