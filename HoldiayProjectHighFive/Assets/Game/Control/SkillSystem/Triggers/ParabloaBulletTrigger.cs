using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Game.Control.Person;
using UnityEngine.Assertions;
using Game.Model;
using UnityEngine;
namespace Game.Control.SkillSystem
{
    class ParabloaBulletTrigger : AbstractSkillTrigger
    {
        private string resName;
        private float damage;
        private float maxLife;
        private float shootSpeed;
        private float timeToTarget;
        private Vector3 offect;

        public override void Init(string args)
        {
            //shootspeed timeToTarget offect.x.y.z damage maxlife 
            var strs = args.Trim().Split('|');
            Assert.IsTrue(strs.Length >= 12);


            this.resName = strs[4].Trim();
            this.shootSpeed = Convert.ToSingle(strs[5].Trim());
            this.timeToTarget = Convert.ToSingle(strs[6].Trim());
            this.offect = new Vector3
            {
                x = Convert.ToSingle(strs[7].Trim()),
                y = Convert.ToSingle(strs[8].Trim()),
                z = Convert.ToSingle(strs[9].Trim())
            };
            this.damage = Convert.ToSingle(strs[10].Trim());
            this.maxLife = Convert.ToSingle(strs[11].Trim());
            base.Init(string.Join("|", strs,0, this.BasePropertyCount));
            
        }
        public override void Execute(AbstractPerson self)
        {
            new ParabloaBullet(this.damage, this.shootSpeed, this.timeToTarget, self.Pos + this.offect,
                self.Pos, self, Const.BulletPath.Dir+this.resName, this.maxLife);
        }
    }
}
