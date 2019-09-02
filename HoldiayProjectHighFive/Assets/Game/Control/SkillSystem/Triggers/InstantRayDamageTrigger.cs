using Game.Control.PersonSystem;
using System;
using Game.Const;
using Game.Math;
using ReadyGamerOne.Data;
using ReadyGamerOne.Script;
using UnityEngine;
using UnityEngine.Assertions;

namespace Game.Control.SkillSystem
{
    /// <summary>
    /// 即时射线伤害触发器，这里会触发击退，音效，锁帧，闪烁，数字等次生效果
    /// </summary>
    public class InstantRayDamageTrigger : AbstractSkillTrigger
    {
        private Vector2 dir;
        private float len;
        private float hitSpeed;
        private LayerMask layer;
        private float shineLastTime;
        private float shineDurTime;
        public override void LoadTxt(string args)//type, int id,float startTime,float lastTime,string args = "")
        {

            this.layer = 1 << LayerMask.NameToLayer("Enemy");
            var strs = args.Split(TxtManager.SplitChar);
            Assert.IsTrue(strs.Length >= BasePropertyCount+5);
            var dre = Convert.ToSingle(strs[BasePropertyCount].Trim());
            this.dir = new Vector2(Mathf.Cos(Mathf.Deg2Rad * dre), Mathf.Sin(Mathf.Deg2Rad * dre));
            //            Debug.Log(this.dir);
            this.len = Convert.ToSingle(strs[BasePropertyCount+1].Trim());
            this.hitSpeed = Convert.ToSingle(strs[BasePropertyCount+2].Trim());
            this.shineLastTime = Convert.ToSingle(strs[BasePropertyCount+3].Trim());
            this.shineDurTime = Convert.ToSingle(strs[BasePropertyCount+4].Trim());
            base.LoadTxt(string.Join(TxtManager.SplitChar.ToString(), strs, 0, this.BasePropertyCount));
        }

        public override void Execute(AbstractPerson self)
        {
            //进行伤害检测，也可以延时检测，实时检测，使用MainLoop中的自定义协程
            //这里进行即时伤害，
            MainLoop.Instance.ExecuteLater(_Execute, this.StartTime, self);
        }
        private void _Execute(AbstractPerson self)
        {
            //临时数据
            var mainc = self.obj.GetComponent<MainCharacter>();
            //面对方向
            var position = self.obj.transform.position;
            var p = new Vector2(position.x, position.y);
            var target = new Vector2(this.dir.x * self.Dir, this.dir.y) * this.len * self.Scanler;

            //调整身高偏移
            p += new Vector2(0, 0.1f * self.obj.transform.localScale.y);
            Debug.DrawLine(p, p + target, Color.red);
            var rescult = Physics2D.LinecastAll(p, p + target, this.layer);

            if (rescult.Length == 0)
            {
                return;
            }


            foreach (var r in rescult)
            {
                //对打击到的目标进行操作，添加各种效果
                var hitPerson = AbstractPerson.GetInstance(r.transform.gameObject);
                if (hitPerson == null)
                {
                    Debug.Log(r.transform.gameObject);
                    continue;
                }


               self.OnCauseDamage(GameMath.Damage(self.Attack));
               hitPerson.TakeBattleEffect(self.AttackEffect);
                
                
                //                hitPerson.TakeBattleEffect(new LockFrame());
            }
        }
        
        public override string Sign
        {
            get
            {
                return TxtSign.instantDamage;
            }
        }
    }
}
