using Game.Const;
using Game.Control.Person;
using Game.Script;
using System;
using UnityEngine;
using UnityEngine.Assertions;
using Game.Model;
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

        public override void Init(string args)
        {
            var strs = args.Split('|');
            Assert.IsTrue(strs.Length >= 9);
            //resName,degree,damage,speed,time
            this.resName = strs[4].Trim();
            var dre = Convert.ToSingle(strs[5].Trim());
            this.dir = new Vector2(Mathf.Cos(Mathf.Deg2Rad * dre), Mathf.Sin(Mathf.Deg2Rad * dre));
            //            Debug.Log("原始方向："+this.dir);
            this.damage = Convert.ToInt32(strs[6].Trim());
            this.speed = Convert.ToInt32(strs[7].Trim());
            this.maxLife = Convert.ToSingle(strs[8].Trim());
            Debug.Log("解析出子弹寿命：" + this.maxLife);
            base.Init(string.Join("|", strs, 0, 4));
        }
        


        public override void Execute(AbstractPerson self)
        {
            this.dir = new Vector2(self.Dir * Mathf.Abs(this.dir.x), this.dir.y);
            Debug.Log(this.maxLife);
            this.bullet=new DirectLineBullet(this.damage, this.dir, self.Pos + new Vector3(0, 0.3f * self.Scanler, 0)
                , self, BulletPath.Dir + this.resName, speed:this.speed,maxLife:this.maxLife);
            
            
        }

    }
}
