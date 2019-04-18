using Game.Control.Person;
using Game.Script;
using UnityEngine;

namespace Game.Model.SpriteObjSystem
{
    public class BulletSpriteObj:AbstractSpriteObj
    {
        private TriggerInputer triggerEvent;
        private float damage;
        private AbstractPerson origin;
        private float maxLife;

        public BulletSpriteObj(float damage,AbstractPerson ap,string path, Vector3 pos, float maxLife,Transform parent = null) : base(path, pos, parent)
        {

            this.maxLife = maxLife;
            if (Mathf.Abs(this.maxLife-0)<0.1f)
            {
                throw new System.Exception("生命时间太短！" + this.maxLife);
            }
            this.damage = damage;
            this.origin = ap;
            if(origin==null)
            {
                throw new System.Exception("子弹发射者为空");
            }
            this.triggerEvent=this.obj.AddComponent<TriggerInputer>();
            this.triggerEvent.onTriggerEnterEvent += OnTriggerEntry;
            this.maxLife = maxLife;
            MainLoop.Instance.ExecuteLater(() =>
            {
                if (this.obj != null)
                {
                    DestoryThis();
                }
            }, this.maxLife);
        }




        private void OnTriggerEntry(Collider2D col)
        {
            var col2d = obj.GetComponent<Collider2D>();
            if (col2d.IsTouchingLayers(1 << LayerMask.NameToLayer("BasicPlatform")))
            {
                Debug.Log("碰到地面");
                DestoryThis();
                return;
            }
            var ap = AbstractPerson.GetInstance(col.gameObject);

            if (ap == null)
                return;

            if (ap is Player && this.origin is Player)
            {
                Debug.Log("两个都是Player");
                return;
            }
            if (!(ap is Player) && !(this.origin is Player))
            {
                Debug.Log("两个都不是Player");
                return;
            }
            

            ap.TakeBattleEffect(this.origin.AttackEffect);
            DestoryThis();
        }


    }
}
