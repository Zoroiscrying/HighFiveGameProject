using Game.Const;
using Game.Control.PersonSystem;
using System;
using Game.Math;
using UnityEngine;
using UnityEngine.Assertions;
using Game.Model.SpriteObjSystem;
using ReadyGamerOne.Data;
using ReadyGamerOne.Script;

namespace Game.Control.SkillSystem
{
    /// <summary>
    /// 远程物体触发器
    /// </summary>
    public class DirectLineBulletTrigger : AbstractSkillTrigger
    {
        private TriggerInputer triggerEvent;
        private string resName;
        private int damage;
        private Vector3 dir;
        private float speed;
        private float maxLife;
        private DirectLineBullet bullet;

        public override void LoadTxt(string args)
        {
            var strs = args.Split(TxtManager.SplitChar);
            Assert.IsTrue(strs.Length >= BasePropertyCount+5);
            //resName,degree,damage,speed,time
            this.resName = strs[BasePropertyCount].Trim();
            var dre = Convert.ToSingle(strs[BasePropertyCount+1].Trim());
            this.dir = new Vector2(Mathf.Cos(Mathf.Deg2Rad * dre), Mathf.Sin(Mathf.Deg2Rad * dre));
            //            Debug.Log("原始方向："+this.dir);
            this.damage = Convert.ToInt32(strs[BasePropertyCount+2].Trim());
            this.speed = Convert.ToInt32(strs[BasePropertyCount+3].Trim());
            this.maxLife = Convert.ToSingle(strs[BasePropertyCount+4].Trim());
//            Debug.Log("解析出子弹寿命：" + this.maxLife);
            base.LoadTxt(string.Join(TxtManager.SplitChar.ToString(), strs, 0, this.BasePropertyCount));
        }
        


        public override void Execute(AbstractPerson self)
        {
            this.dir = new Vector2(self.Dir * Mathf.Abs(this.dir.x), this.dir.y);
            Debug.Log(this.maxLife);
            this.bullet=new DirectLineBullet(GameMath.Damage(self.Attack), this.dir, self.Pos + new Vector3(0, 0.3f * self.Scanler, 0)
                , self, DirPath.BulletDir + this.resName, speed:this.speed,maxLife:this.maxLife);
            
            
        }

        
        public override string Sign
        {
            get
            {
                return TxtSign.bullet;
            }
        }
    }
}
