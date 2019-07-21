using Game.Control.PersonSystem;
using Game.Script;
using System;
using Game.Const;
using Game.Data;
using UnityEngine;
using UnityEngine.Assertions;

namespace Game.Control.SkillSystem
{
    /// <summary>
    /// 刀剑类碰撞体触发器
    /// </summary>
    public class SwordTrigger : AbstractSkillTrigger
    {
        private Vector2 personOffect;
        private Vector2 size;
        private float beginDre;
        private float endDre;

        private GameObject empty;
        private AbstractPerson self;
        private float nowZ;
        private float speed;
        public override void LoadTxt(string args)
        {
            //args
            //offect.x,offect.y,size.x,size.y,beginDre,endDre,shineLastTime,shineDurTime,hitSpeed
            var strs = args.Split(TxtManager.SplitChar);
            Assert.IsTrue(strs.Length >= BasePropertyCount+6);
            this.personOffect = new Vector2(
                Convert.ToSingle(strs[BasePropertyCount].Trim()),
                Convert.ToSingle(strs[BasePropertyCount+1].Trim()));
            this.size = new Vector2(
                Convert.ToSingle(strs[BasePropertyCount+2].Trim()),
                Convert.ToSingle(strs[BasePropertyCount+3].Trim()));
            this.beginDre = Convert.ToSingle(strs[BasePropertyCount+4].Trim());
            this.endDre = Convert.ToSingle(strs[BasePropertyCount+5].Trim());
            base.LoadTxt(string.Join(TxtManager.SplitChar.ToString(), strs, 0, this.BasePropertyCount));
        }

        public override void Execute(AbstractPerson self)
        {
            this.self = self;
            this.LastTime /= self.AttackSpeed;
            this.speed = (endDre - beginDre) / this.LastTime;

            empty = new GameObject();
            empty.transform.SetParent(self.obj.transform);
            empty.transform.localScale = this.size;
            empty.transform.localPosition = this.personOffect;
            empty.gameObject.layer = LayerMask.NameToLayer("Trigger");
            var trigger = empty.AddComponent<BoxCollider2D>();
            trigger.isTrigger = true;
            trigger.offset = new Vector2(0.5f, -1);
            var triggerEvent = empty.AddComponent<TriggerInputer>();
            triggerEvent.onTriggerEnterEvent += OnTriggerEnter;
            var r = empty.transform.rotation;
            r = Quaternion.Euler(r.x, r.y, this.beginDre);

            MainLoop.Instance.UpdateForSeconds(Update, this.LastTime, self, End);
        }

        private void Update(AbstractPerson ap)
        {
            if (empty == null)
            {
                Debug.Log("伤害检测物体为空");
                return;
            }
            empty.transform.Rotate(new Vector3(0, 0, this.speed));
        }

        private void End(AbstractPerson ap)
        {
            if (this.empty)
                UnityEngine.Object.Destroy(this.empty);
        }

        private void OnTriggerEnter(Collider2D col)
        {
            var hitPerson = AbstractPerson.GetInstance(col.gameObject);
            if (hitPerson == null)
            {
                Debug.Log("打击人物为空");
                return;
            }
            hitPerson.TakeBattleEffect(self.AttackEffect);
        }

        public override void Release()
        {
            base.Release();
            End(this.self);
        }
        
        public override string Sign
        {
            get
            {
                return TxtSign.trigger2D;
            }
        }
    }
}
