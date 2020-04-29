using HighFive.Const;
using HighFive.Model.Person;
using ReadyGamerOne.Script;
using UnityEngine;

namespace HighFive.Model.SpriteObjSystem
{
    public class DirectLineBullet : BulletSpriteObj
    {
        private TriggerInputer triggerEvent;
        private int damage;
        private Vector3 dir;
        private float speed;

        public DirectLineBullet(int damage, Vector3 dir, Vector3 pos, IHighFivePerson origin, string path,float speed = 8f, float maxLife=6f, Transform parent = null)
            : base(damage,origin,path, pos,maxLife, parent)
        {
            this.dir = dir;
            this.speed = speed;
            AudioMgr.Instance.PlayEffect(AudioName.beiji);
        }
        protected override void Update()
        {
            base.Update();

            //子弹飞行
            this.obj.transform.Translate(this.dir.normalized * Time.deltaTime * this.speed);

        }



    }
}
