using Game.Control.Person;
using Game.Script;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Game.Model
{
    public class Bullet : AbstractSpriteObj
    {
        private TriggerInputer triggerEvent;
        private int damage;
        private int dir;
        private float speed;
        private AbstractPerson origin;

        public Bullet(int damage, int dir, Vector3 pos, AbstractPerson origin, float speed = 8f, Transform parent = null)
            : base(Const.BulletPath.PlayerBullet, pos, parent)
        {
            this.origin = origin;
            this.damage = damage;
            this.dir = dir;
            this.speed = speed;
        }

        private void OnTriggerEntry(Collider2D col)
        {
            var col2d = go.GetComponent<Collider2D>();
            if (col2d.IsTouchingLayers(1 << LayerMask.NameToLayer("BasicPlatform")))
            {
                DestoryThis();
                return;
            }
            var ap = AbstractPerson.GetInstance(col.gameObject);

            if (ap == null)
                return;

            if (ap is Player && this.origin is Player)
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
            this.go.transform.Translate(Vector3.right * this.dir * Time.deltaTime * this.speed);

        }

        public override void Init()
        {
            base.Init();

            this.triggerEvent = this.go.AddComponent<TriggerInputer>();
            this.triggerEvent.onTriggerEnterEvent += OnTriggerEntry;

            AudioMgr.Instance.PlayAuEffect("beiji", this.go.transform.position);
            MainLoop.Instance.ExecuteLater(DestoryThis, 4);
        }

        public override void Release()
        {
            base.Release();
            this.triggerEvent.onTriggerEnterEvent -= OnTriggerEntry;
        }

    }
}
