using Game.Const;
using Game.Control.Person;
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
    /// 远程物体触发器
    /// </summary>
    public class BulletTrigger : AbstractSkillTrigger
    {
        private TriggerInputer triggerEvent;
        private string resName;
        private int damage;
        private Vector2 dir;
        private float speed;
        private GameObject go;
        private float time;
        private AbstractPerson origin;

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
            this.time = Convert.ToSingle(strs[8].Trim());
            base.Init(string.Join("|", strs, 0, 4));
        }

        protected virtual void OnTriggerEntry(Collider2D col)
        {
            //            Debug.Log("发射者"+this.origin.name+" "+"碰到"+col.name);
            var ap = AbstractPerson.GetInstance(col.gameObject);

            if (ap == null)
                return;
            if (ap is Player && this.origin is Player)
                return;
            if (!(ap is Player) && !(this.origin is Player))
                return;

            //            Debug.Log(ap.name);
            //如果碰到人物，产生效果和次生效果
            ap.TakeBattleEffect(this.origin.AttackEffect);
            this.DestoryThis();
        }


        public override void Execute(AbstractPerson self)
        {
            this.origin = self;
            this.dir = new Vector2(self.Dir * Mathf.Abs(this.dir.x), this.dir.y);

            //实例化
            var res = Resources.Load<GameObject>(BulletPath.Dir + this.resName);
            if (res == null)
                Debug.LogError("图片路径错误");
            this.go = GameObject.Instantiate(res, self.Pos + new Vector3(0, 0.3f * self.Scanler, 0), Quaternion.identity);

            Assert.IsTrue(go != null);

            //添加组件
            this.triggerEvent = this.go.AddComponent<TriggerInputer>();
            this.triggerEvent.onTriggerEnterEvent += OnTriggerEntry;

            //开始循环
            MainLoop.Instance.UpdateForSeconds(Update, this.time, DestoryThis);

        }


        protected virtual void Update()
        {
            if (this.go == null)
            {
                Debug.Log("子弹空了");
                return;
            }
            this.go.transform.Translate(Vector3.right * this.dir * Time.deltaTime * this.speed);
        }

        protected virtual void DestoryThis()
        {
            if (this.go == null)
                return;
            GameObject.Destroy(this.go);
            this.go = null;
        }

        public override void Release()
        {
            base.Release();
            DestoryThis();
        }
    }
}
