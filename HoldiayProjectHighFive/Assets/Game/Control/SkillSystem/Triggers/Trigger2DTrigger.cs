﻿using Game.Control.Person;
using Game.Script;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.Assertions;

namespace Game.Control.SkillSystem
{
    /// <summary>
    /// 碰撞体触发器
    /// </summary>
    public class Trigger2DTrigger : AbstractSkillTrigger
    {
        private Vector2 personOffect;
        private Vector2 size;
        private float beginDre;
        private float endDre;

        private GameObject empty;
        private AbstractPerson self;
        private float nowZ;
        public override void Init(string args)
        {
            //args
            //offect.x,offect.y,size.x,size.y,beginDre,endDre,shineLastTime,shineDurTime,hitSpeed
            var strs = args.Split('|');
            Assert.IsTrue(strs.Length >= 10);
            this.personOffect = new Vector2(
                Convert.ToSingle(strs[4].Trim()),
                Convert.ToSingle(strs[5].Trim()));
            this.size = new Vector2(
                Convert.ToSingle(strs[6].Trim()),
                Convert.ToSingle(strs[7].Trim()));
            this.beginDre = Convert.ToSingle(strs[8].Trim());
            this.endDre = Convert.ToSingle(strs[9].Trim());
            base.Init(string.Join("|", strs, 0, 4));
        }

        public override void Execute(AbstractPerson self)
        {
            this.self = self;
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
                Debug.Log("empty为空");
                return;
            }
            var r = empty.transform.rotation;
            float z = Mathf.Lerp(r.z, endDre, 0.5f);
            r = Quaternion.Euler(r.x, r.y, z);
        }

        private void End(AbstractPerson ap)
        {
            if (this.empty)
                GameObject.Destroy(this.empty);
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
    }
}