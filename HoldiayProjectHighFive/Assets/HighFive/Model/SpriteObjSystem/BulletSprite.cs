using HighFive.Model.Person;
using ReadyGamerOne.Rougelike.Person;
using ReadyGamerOne.Script;
using UnityEngine;

namespace HighFive.Model.SpriteObjSystem
{
    public class BulletSpriteObj:AbstractSpriteObj
    {
        private TriggerInputer triggerEvent;
        private int damage;
        private IHighFivePerson origin;
        private float maxLife;

        public BulletSpriteObj(int damage,IHighFivePerson ap,string path, Vector3 pos, float maxLife,Transform parent = null) : base(path, pos, parent)
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
            this.triggerEvent.onTriggerEnterEvent2D += OnTriggerEntry;
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
            if (col2d.IsTouchingLayers(1<<LayerMask.NameToLayer("BasicPlatform")))
            {
//                Debug.Log("碰到地面");
                DestoryThis();
                return;
            }
            var ap = col.gameObject.GetPersonInfo() as IHighFivePerson;

            if (ap == null)
                return;

            if (ap is IHighFiveCharacter && this.origin is IHighFiveCharacter)
            {
//                Debug.Log($"两个都是Player:[{ap.CharacterName}][{origin.CharacterName}]");
                return;
            }
            if (!(ap is IHighFiveCharacter) && !(this.origin is IHighFiveCharacter))
            {
//                Debug.Log($"两个都不是Player:[{ap.CharacterName}][{origin.CharacterName}]");
                return;
            }

//            Debug.Log(this.origin.CharacterName+" 攻击 "+ap.characterInfoInfo);

            this.origin.TryAttack(ap, damage);
            ap.PlayAcceptEffects(origin as IHighFivePerson);
            DestoryThis();
        }


    }
}
