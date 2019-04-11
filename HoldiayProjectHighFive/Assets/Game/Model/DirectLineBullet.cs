using Game.Control.Person;
using Game.Script;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Game.Model
{
    public class DirectLineBullet : BulletSpriteObj
    {
        private TriggerInputer triggerEvent;
        private int damage;
        private Vector3 dir;
        private float speed;
        private AbstractPerson origin;

        public DirectLineBullet(int damage, Vector3 dir, Vector3 pos, AbstractPerson origin, string path,float speed = 8f, float maxLife=6f, Transform parent = null)
            : base(damage,origin,path, pos,maxLife, parent)
        {
            this.dir = dir;
            this.speed = speed;
            AudioMgr.Instance.PlayAuEffect("beiji", this.obj.transform.position);
        }
        protected override void Update()
        {
            base.Update();

            //子弹飞行
            this.obj.transform.Translate(this.dir.normalized * Time.deltaTime * this.speed);

        }



    }
}
