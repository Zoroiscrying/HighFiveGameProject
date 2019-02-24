using System;
using System.Collections;
using System.Collections.Generic;
using Game.Control;
using UnityEngine;
using Game;
using Game.Script;

namespace Game.Modal
{
    public class Bullet:BaseSpriteGO
    {
        private TriggerEvent triggerEvent;
        private int damage;
        private int dir;
        private float speed;
        private AbstractPerson origin;
        
        public Bullet(int damage,int dir,Vector3 pos,AbstractPerson origin,float speed=8f, Transform parent = null) 
            : base(Const.BulletPath.PlayerBullet, pos, parent)
        {
            this.origin = origin;
            this.damage = damage;
            this.dir = dir;
            this.speed = speed;
        }

        private void OnTriggerEntry(Collider2D col)
        {
            
            var ap = AbstractPerson.GetInstance(col.gameObject);

            if (ap == null)
                return;
            
            if(ap is Player&&this.origin is Player)
                return;
            if (!(ap is Player) && !(this.origin is Player))
                return;
            
//            Debug.Log("发射者"+this.origin.name+" "+"碰到"+ap.name);
//            Debug.Log(ap.name);
            //如果碰到人物，产生效果和次生效果
            ap.TakeBattleEffect(this.origin.AttackEffect);
            DestoryThis();
        }
        
        protected override void Update()
        {
            base.Update();
            
            //子弹飞行
            this.go.transform.Translate(Vector3.right*this.dir*Time.deltaTime*this.speed);
            
        }

        public override void Init()
        {
            base.Init();
            
            this.triggerEvent=this.go.AddComponent<TriggerEvent>();
            this.triggerEvent.onTriggerEnterEvent +=OnTriggerEntry;

            AudioMgr.Instance.PlayAuEffect("beiji",this.go.transform.position);
            MainLoop.Instance.ExecuteLater(DestoryThis, 4);
        }

        public override void Release()
        {
            base.Release();
            this.triggerEvent.onTriggerEnterEvent -=OnTriggerEntry;
        }

    }
}
