using Game.Control.PersonSystem;
using System;
using System.IO;
using Game.Const;
using Game.Data;
using UnityEngine;
using UnityEngine.Assertions;

namespace Game.Control.SkillSystem
{
    /// <summary>
    /// 动画触发器
    /// </summary>
    public class AnimationTrigger : AbstractSkillTrigger,ITxtSerializable
    {
        private string animationName;
        private float speed;

        /// <summary>
        /// 初始化子类，这里要通过从args中获取信息，完成子类特有信息的初始化
        /// </summary>
        /// <param name="type"></param>
        /// <param name="args"></param>
        public override void LoadTxt(string args)//type,int id,float startTime,float lastTime, string args)
        {
            var strs = args.Split(TxtManager.SplitChar);
            Assert.IsTrue(strs.Length >= BasePropertyCount+2);
            this.animationName = strs[BasePropertyCount+0].Trim();
            this.speed = Convert.ToSingle(strs[BasePropertyCount+1].Trim());
            this.LastTime /= this.speed;
            base.LoadTxt(string.Join(TxtManager.SplitChar.ToString(), strs, 0, this.BasePropertyCount));
        }

        /// <summary>
        /// 触发函数
        /// </summary>
        /// <param name="self">挂载每个AbstractPerson身上的脚本，也可以识别的东西，这里以后可以换</param>
        /// <returns></returns>
        public override void Execute(AbstractPerson self)
        {
            //触发器执行具体代码
            //这里也可以由延时调用，比如技能开始 0.15s后开始动画/音效/位移，

            if (self == null)
            {
                throw new Exception("SkillCore为空");
            }

            var animator = GameAnimator.GetInstance(self.obj.GetComponent<Animator>());

            if (animator == null)
            {
                throw new Exception(self.name + "没有Animator组件");
            }

            animator.speed = this.speed*self.AttackSpeed;

            
            //            Debug.Log("播放动画了");
            animator.Play(Animator.StringToHash(this.animationName), AnimationWeight.High);
        }

        public override string Sign
        {
            get { return DataSign.animation; }
        }


    }
}
