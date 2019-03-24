using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection.Emit;
using System.Runtime.InteropServices;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using Game;
using Game.Control;
using Game.Const;
using Game.Script;
using Game.View;
using UnityEngine.Assertions;
using Game.Serialization;
using Random = UnityEngine.Random;



namespace Game.Control
{
    ////////////////////////////////     战斗效果系统    ////////////////////////////////
    
    /// <summary>
    /// 所有战斗效果（恢复，伤害，治疗，Buff）的基类
    /// </summary>
    public interface IBattleEffect
    {
        /// <summary>
        /// 发挥作用
        /// </summary>
         void Execute(AbstractPerson ap);
    }

    public abstract class BattleEffect : IBattleEffect
    {
        public abstract void Execute(AbstractPerson ap);

        public virtual void Release(AbstractPerson ap)
        {
            ap.bfList.Remove(this);
        }
    }
    
    
    /// <summary>
    /// 即时扣血效果,包括伤害数字的现实
    /// </summary>
    public class InstantDamage : BattleEffect
    {
        private int damage;
        private int dir;

        public InstantDamage(int damage,int dir)
        {
            this.dir = dir;
            this.damage = damage;
        }

        public override void Execute(AbstractPerson ap)
        {
            
//            Debug.Log(ap.obj.GetInstanceID()+": "+ap.IsConst);
            //如果不可选定（硬直）,告辞
            //数值
//            Debug.Log( ap.name + "收到" + this.damage + "伤害");

            //   BloodChange
            
            
            if(!ap.IsConst)
                ap.TakeBattleEffect(new DamageNumber(this.damage,0.3f*ap.Scanler,Color.red,this.dir,1f,Random.Range(10,60)));

            CEventCenter.BroadMessage(Message.M_BloodChange(ap.obj),-this.damage);
            this.Release(ap);
        }
    }
    
    
    /// <summary>
    /// 持续回血效果，这也是个例子
    /// </summary>
    public class RecoverBuff : BattleEffect
    {
        private int amount;
        private int times;
        private float duringTime;

        public RecoverBuff(int amount, int times, float duringTime)
        {
            this.amount = amount;
            this.times = times;
            this.duringTime = duringTime;
        }

        public override void Execute(AbstractPerson ap)
        {
            MainLoop.Instance.ExecuteEverySeconds(_Recover,this.times,this.duringTime,ap,Release);
        }
        private void _Recover(AbstractPerson ap)
        {
            CEventCenter.BroadMessage(Message.M_BloodChange(ap.obj),this.amount);
        }
    }

    
    ////////////////////////////////      次生效果     //////////////////////////////////
    
    /// <summary>
    /// 击退效果
    /// </summary>
    public class Hitback : BattleEffect
    {
        private Vector2 hit;

        public Hitback(Vector2 hit)
        {
            this.hit = hit;
        }
        public override void Execute(AbstractPerson ap)
        {
            #region 加力
//            var rig = ap.obj.GetComponent<Rigidbody2D>();
//            if(rig==null)
//                Debug.LogError(ap.name+"没有Rigidbody2D");
//            rig.AddForce(new Vector2(this.dir*this.InstantSpeed,0));
            # endregion

            #region Transform

            if (!ap.IgnoreHitback)
            {   
                var trans = ap.obj.transform;
                trans.position += new Vector3(this.hit.x, this.hit.y, 0);
            }

            #endregion
            
            this.Release(ap);
        }
    }

    
    /// <summary>
    /// 闪烁效果
    /// </summary>
    public class Shine : BattleEffect
    {
        private SpriteRenderer sr;
        private bool flag;
        private float lastTime;
        private float durTime;
        private Color color;
        private AbstractPerson ap;

        public Shine(float lastTime, float durTime, Color color) 
        {
            this.flag = false;
            this.lastTime = lastTime;
            this.durTime = durTime;
            this.color = color;
        }
        public override void Execute(AbstractPerson ap)
        {
            this.ap = ap;
            sr = ap.obj.GetComponent<SpriteRenderer>();
            int x = (int) (this.lastTime / this.durTime);
            int n = x % 2 == 0 ? x + 1: x ;
            
            MainLoop.Instance.ExecuteEverySeconds(_Execute, n, this.durTime,_Reset);
        }

        private void _Execute()
        {
            if (this.sr == null)
                return;

            if (this.flag)
                this.sr.color = Color.white;
            else
                this.sr.color = this.color;
            this.flag = !this.flag;
        }

        private void _Reset()
        {
            this.Release(this.ap);
            if (sr == null)
                return;
            sr.color = Color.white;
        }
    }

    
    /// <summary>
    /// 音效
    /// </summary>
    public class AudioEffect : BattleEffect
    {
        private string audioName;

        public AudioEffect(string audioName)
        {
            this.audioName = audioName;
        }
        
        public override void Execute(AbstractPerson ap)
        {
            AudioMgr.Instance.PlayAuEffect(this.audioName,ap.obj.transform.position);
            this.Release(ap);
        }
    }


    /// <summary>
    /// 数字效果
    /// </summary>
    public class DamageNumber : BattleEffect
    {
        private int damageNum;
        private float time;
        private int size;
        private Color color;
        private float yOffect;
        private int dir;
        public DamageNumber(int damageNum, float yOffect,Color color,int dir,float time=1f,int size =24)
        {
            this.damageNum = damageNum;
            this.size = size;
            this.time = time;
            this.color = color;
            this.yOffect = yOffect;
            this.dir = dir;
        }

        public override void Execute(AbstractPerson ap)
        {
            new NumberTip(this.damageNum, this.yOffect,this.size, this.color, ap.obj.transform,this.dir,this.time);
            this.Release(ap);
        }
    }


    /// <summary>
    /// 锁帧效果
    /// </summary>
    public class LockFrame : BattleEffect
    {
        private float time;

        public LockFrame(float time = 0.04f)
        {
            this.time = time;
        }

        public override void Execute(AbstractPerson ap)
        {
            Time.timeScale = 0;
            MainLoop.Instance.ExecuteLater(_Reset,this.time,ap);
        }

        private void _Reset(AbstractPerson ap)
        {
            Time.timeScale = 1;
            this.Release(ap);
        }
    }


    /// <summary>
    /// 震屏效果
    /// </summary>
    public class ShakeScreen : BattleEffect
    {
        private float howmuch;
        private float time;

        public ShakeScreen(float howmuch, float time)
        {
            this.howmuch = howmuch;
            this.time = time;
        }

        public override void Execute(AbstractPerson ap)
        {
            ScreenShake.Instance.enabled = false;
            ScreenShake.Instance.enabled = true;

            ScreenShake.Instance.shakeDir = this.howmuch*Vector3.one;
            ScreenShake.Instance.shakeTime = this.time;
            this.Release(ap);
        }
    }
    
    
    ////////////////////////////////     技能/触发器 系统    /////////////////////////////
    
    /// <summary>
    /// 触发器接口
    /// </summary>
    public interface ISkillTrigger
    {
        float StartTime { get; set; }
        string SkillType { get; set; }
        float LastTime { get; set; }
        
        int id { get; set; }

        void Init(string args);
//        void Init(string type,int id,float startTime,float lastTime,string args);
        void Release();
        void Execute(AbstractPerson self);
    }
    
    
    /// <summary>
    /// 技能触发器抽象类
    /// </summary>
    public abstract class AbstractSkillTrigger : ISkillTrigger
    {
        public float LastTime { get; set; }
        public int id { get; set; }
        public float StartTime { get; set; }
        public string SkillType { get; set; }
        public bool IsExecuted { get; set; }
        
        public virtual void Release()
        {
            this.IsExecuted = false;
        }


        /// <summary>
        /// 这里要完成startTime,lastTime，SkillType,IsExecuted以及其他额外参数的初始化
        /// </summary>
        /// <param name="type"></param>
        /// <param name="args"></param>
        public virtual void Init(string args)//string type, int id,float startTime,float lastTime,string args="")
        {
            var strs = args.Split('|');
            Assert.IsTrue(strs.Length>=4);
            this.IsExecuted = false;
            this.LastTime = Convert.ToSingle(strs[3].Trim());
            this.StartTime = Convert.ToSingle(strs[1].Trim());
            this.id = Convert.ToInt32(strs[2].Trim());
            this.SkillType = strs[0].Trim();
        }
        
        /// <summary>
        /// 触发函数
        /// </summary>
        /// <param name="self"></param>
        /// <returns></returns>
        public abstract void Execute(AbstractPerson self);
    }
    
    
    /// <summary>
    /// 动画触发器
    /// </summary>
    public class AnimationTrigger : AbstractSkillTrigger
    {
        private string animationName;
        private float speed;
        
        /// <summary>
        /// 初始化子类，这里要通过从args中获取信息，完成子类特有信息的初始化
        /// </summary>
        /// <param name="type"></param>
        /// <param name="args"></param>
        public override void Init(string args)//type,int id,float startTime,float lastTime, string args)
        {
            var strs = args.Split('|');
            Assert.IsTrue(strs.Length>=6);
            this.animationName = strs[4].Trim();
            this.speed = Convert.ToSingle(strs[5].Trim());
            this.LastTime /= this.speed;
            base.Init(string.Join("|", strs, 0, 4));
        }
        
        /// <summary>
        /// 触发函数
        /// </summary>
        /// <param name="self">挂载每个AbstractPerson身上的脚本，也可以识别的东西，这里以后可以换</param>
        /// <returns></returns>
        public override void Execute(AbstractPerson self)
        {
            //触发器执行具体代码
            //这里也可以由延时调用，比如技能开始 0.15s后开始动画/音效/位移，
            
            if (self == null)
            {
                throw new Exception("SkillCore为空");
            }

            var animator =GameAnimator.GetInstance(self.obj.GetComponent<Animator>());

            if (animator == null)
            {
                throw new Exception(self.name+"没有Animator组件");
            }
            animator.speed = this.speed;
//            Debug.Log("播放动画了");
            animator.Play(Animator.StringToHash(this.animationName),AnimationWeight.High);
        }
    }

    
//    /// <summary>
//    /// 锁帧触发器
//    /// </summary>
//    public class LockFrameTrigger : AbstractSkillTrigger
//    {
//        
//
//        public override void Execute(AbstractPerson self)
//        {
//            Time.timeScale = 0;
//            MainLoop.Instance.ExecuteLater(_Reset,this.LastTime);
//        }
//
//        private void _Reset()
//        {
//            Time.timeScale = 1;
//        }
//    }
    
    
    /// <summary>
    /// 技能位移触发器
    /// </summary>
    public class DashTrigger : AbstractSkillTrigger
    {
        private MainCharacter controller;
        private float StartSpeed;
        private float EndSpeed;
        private float dur;
            
        
        public override void Execute(AbstractPerson self)
        {
            this.controller=self.obj.gameObject.GetComponent<MainCharacter>();
            MainLoop.Instance.ExecuteLater(_SetSpeed, this.StartTime, self);
            MainLoop.Instance.UpdateForSeconds(_Execute,this.LastTime,self,this.StartTime);
        }

        private void _SetSpeed(AbstractPerson self)
        {
            controller._playerVelocityX = self.Dir*this.StartSpeed;
        }

        private void _Execute(AbstractPerson self)
        {
            controller._playerVelocityX =self.Dir* Mathf.Lerp(Mathf.Abs(controller._playerVelocityX), this.EndSpeed, this.dur);
        }

        public override void Init(string args)//type, int id,float startTime, float lastTime, string args = "")
        {
            var strs = args.Split('|');
            Assert.IsTrue(strs.Length >= 7);
            this.StartSpeed = Convert.ToSingle(strs[4].Trim());
            this.EndSpeed = Convert.ToSingle(strs[5].Trim());
            this.dur = Convert.ToSingle(strs[6].Trim());
            if (dur <= 0 || dur >= 1)
                Debug.LogError("差值不合理 " +dur);
            base.Init(string.Join("|", strs, 0, 4));
        }
    }


    /// <summary>
    /// 音效触发器
    /// </summary>
    public class AudioTrigger : AbstractSkillTrigger
    {
        private string audioName;

        public override void Init(string args)//type, int id,float startTime, float lastTime, string args = "")
        {
            var strs = args.Split('|');
            Assert.IsTrue(strs.Length >= 5);
            this.audioName = strs[4].Trim();
            base.Init(string.Join("|", strs, 0, 4));
        }

        public override void Execute(AbstractPerson self)
        {
            MainLoop.Instance.ExecuteLater(_Execute, this.StartTime, self);
        }

        private void _Execute(AbstractPerson ap)
        {
            AudioMgr.Instance.PlayAuEffect(this.audioName, ap.obj.transform.position);

        }
    }
    
    
    /// <summary>
    /// 即时射线伤害触发器，这里会触发击退，音效，锁帧，闪烁，数字等次生效果
    /// </summary>
    public class InstantRayDamageTrigger : AbstractSkillTrigger
    {
        private Vector2 dir;
        private float len;
        private float hitSpeed;
        private LayerMask layer;
        private float shineLastTime;
        private float shineDurTime;
        public override void Init(string args)//type, int id,float startTime,float lastTime,string args = "")
        {
            
            this.layer = 1<<LayerMask.NameToLayer("Enemy");
            var strs = args.Split('|');
            var dre = Convert.ToSingle(strs[4].Trim());
            Assert.IsTrue(strs.Length >=9);
            this.dir = new Vector2(Mathf.Cos(Mathf.Deg2Rad*dre),Mathf.Sin(Mathf.Deg2Rad*dre));
//            Debug.Log(this.dir);
            this.len = Convert.ToSingle(strs[5].Trim());
            this.hitSpeed = Convert.ToSingle(strs[6].Trim());
            this.shineLastTime = Convert.ToSingle(strs[7].Trim());
            this.shineDurTime = Convert.ToSingle(strs[8].Trim());
            base.Init(string.Join("|", strs, 0, 4));
        }

        public override void Execute(AbstractPerson self)
        {
            //进行伤害检测，也可以延时检测，实时检测，使用MainLoop中的自定义协程
            //这里进行即时伤害，
            MainLoop.Instance.ExecuteLater(_Execute,this.StartTime,self);
        }
        private void _Execute(AbstractPerson self)
        {
            //临时数据
            var mainc = self.obj.GetComponent<MainCharacter>();
            //面对方向
            var position = self.obj.transform.position;
            var p = new Vector2(position.x, position.y);
            var target=new Vector2(this.dir.x*self.Dir,this.dir.y)*this.len*self.Scanler;

            //调整身高偏移
            p+=new Vector2(0,0.1f*self.obj.transform.localScale.y);
            Debug.DrawLine(p, p+target, Color.red);
            var rescult = Physics2D.LinecastAll(p,p+target,this.layer);

            if (rescult.Length == 0)
            {
                return;
            }

            
            foreach (var r in rescult)
            {
                //对打击到的目标进行操作，添加各种效果
                var hitPerson = AbstractPerson.GetInstance(r.transform.gameObject);
                if (hitPerson == null)
                {
                    Debug.Log(r.transform.gameObject);
                    continue;
                }

//                Debug.Log(self.name + "将对" + hitPerson.name + "造成" + GameMath.Damage(self, hitPerson) + "伤害");
                hitPerson.TakeBattleEffect(self.AttackEffect);
//                hitPerson.TakeBattleEffect(new LockFrame());
            }
        }
    }


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
            Assert.IsTrue(strs.Length>=10);
            this.personOffect = new Vector2(
                Convert.ToSingle(strs[4].Trim()),
                Convert.ToSingle(strs[5].Trim()));
            this.size=new Vector2(
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
            empty.transform.localScale =this.size ;
            empty.transform.localPosition = this.personOffect;
            empty.gameObject.layer =LayerMask.NameToLayer("Trigger");
            var trigger=empty.AddComponent<BoxCollider2D>();
            trigger.isTrigger = true;
            trigger.offset = new Vector2(0.5f, -1);
            var triggerEvent=empty.AddComponent<TriggerEvent>();
            triggerEvent.onTriggerEnterEvent += OnTriggerEnter;
            var r = empty.transform.rotation;
            r = Quaternion.Euler(r.x, r.y, this.beginDre);
            
            MainLoop.Instance.UpdateForSeconds(Update,this.LastTime,self,End);
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
            if(this.empty)
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
    
    
    /// <summary>
    /// 远程物体触发器
    /// </summary>
    public class BulletTrigger:AbstractSkillTrigger
    {
        private TriggerEvent triggerEvent;
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
            Assert.IsTrue(strs.Length>=9);
            //resName,degree,damage,speed,time
            this.resName = strs[4].Trim();
            var dre = Convert.ToSingle(strs[5].Trim());
            this.dir = new Vector2(Mathf.Cos(Mathf.Deg2Rad*dre),Mathf.Sin(Mathf.Deg2Rad*dre));
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
            if(ap is Player&&this.origin is Player)
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
            var res = Resources.Load<GameObject>(BulletPath.Dir+this.resName);
            if (res == null)
                Debug.LogError("图片路径错误");
            this.go=GameObject.Instantiate(res, self.Pos+new Vector3(0,0.3f*self.Scanler,0), Quaternion.identity);
            
            Assert.IsTrue(go != null);
            
            //添加组件
            this.triggerEvent=this.go.AddComponent<TriggerEvent>();
            this.triggerEvent.onTriggerEnterEvent +=OnTriggerEntry;
            
            //开始循环
            MainLoop.Instance.UpdateForSeconds(Update,this.time,DestoryThis);
            
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


    /////////////////////////////////     触发器/技能 的创建及管理     /////////////////////////
    
    /// <summary>
    /// 触发器工厂接口
    /// </summary>
    public interface ISkillTriggerFactory
    {
        ISkillTrigger CreateTrigger(string args); //type, int id,float startTime,float lastTime,string args);
    }
    
    
    /// <summary>
    /// 触发器抽象模板工厂类
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class SkillTriggerFactory<T>:Singleton<SkillTriggerFactory<T>>,ISkillTriggerFactory
        where T : ISkillTrigger,new()
    {
        public ISkillTrigger CreateTrigger(string args)//type,int id,float startTime,float lastTime,string args)
        {
            var t = new T();
            t.Init(args);//type,id,startTime,lastTime,args);
            return t;
        }
    }
    
    
    /// <summary>
    /// 触发器管理者类
    /// </summary>
    public class SkillTriggerMgr : Singleton<SkillTriggerMgr>
    {
        private SerializableDictionary<string,ISkillTriggerFactory> factoryDic=new SerializableDictionary<string, ISkillTriggerFactory>();
        
        /// <summary>
        /// 注册不同种类的触发器
        /// </summary>
        /// <param name="skillType"></param>
        /// <param name="factory"></param>
        public void RegisterTriggerFactory(string skillType, ISkillTriggerFactory factory)
        {
            if(!factoryDic.ContainsKey(skillType))
                factoryDic.Add(skillType,factory);
        }
        
        /// <summary>
        /// 创建触发器
        /// </summary>
        /// <param name="skillType"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public ISkillTrigger CreateTrigger(string args)//skillType,int id,float startTime, float lastTime,string args)
        {
            var strs = args.Split('|');
            var type = strs[0].Trim();
            if(!factoryDic.ContainsKey(type))
                throw new Exception("工厂中没有这个触发器类型");
            return factoryDic[type].CreateTrigger(args);//skillType,id,startTime,lastTime,args);
        }
        
    }
    
    
    /// <summary>
    /// 技能实例类，一个具体的技能对应一个这个类的实例
    /// 技能由多个触发器构成
    /// </summary>
    public class SkillInstance
    {
        public event Action<SkillInstance> onSkillBegin=null;
        public event Action<SkillInstance> onSkillExit=null;
        public float startTime;
        public string name;
        private bool isUsed = false;
        private float lastTime = 0f;//是否正在使用
        private List<ISkillTrigger> skillTriggers=new List<ISkillTrigger>();//所有触发器

        public SkillInstance(string name)
        {
            this.name = name;
        }
        public float LastTime
        {
            get { return lastTime; }
        }

        public int TriggerCount
        {
            get { return skillTriggers.Count; }
        }
        
        /// <summary>
        /// 添加触发器
        /// </summary>
        /// <param name="trigger"></param>
        public void AddTrigger(ISkillTrigger trigger)
        {
            if (trigger.StartTime + trigger.LastTime > this.lastTime)
                this.lastTime = trigger.StartTime + trigger.LastTime;
            skillTriggers.Add(trigger);
        }
        
        /// <summary>
        /// 移除触发器
        /// </summary>
        /// <param name="trigger"></param>
        public void RemoveTrigger(ISkillTrigger trigger)
        {
            skillTriggers.Remove(trigger);
            this.lastTime = 0f;
            foreach (var t in skillTriggers)
            {
                if (trigger.StartTime + trigger.LastTime > this.lastTime)
                    this.lastTime = trigger.StartTime + trigger.LastTime;
            }
        }
        
        /// <summary>
        /// 刷新技能
        /// </summary>
        private void Reset()
        {
            foreach(var trigger in skillTriggers)
                trigger.Release();
            if (onSkillExit != null)
                onSkillExit(this);
            this.isUsed = false;
        }
        
        /// <summary>
        /// 触发函数
        /// </summary>
        /// <param name="self">使用技能的单位</param>
        /// <param name="ignoreInput">是否屏蔽技能输入</param>
        public void Execute(AbstractPerson self,bool ignoreInput=false,float startTime=0)
        {
            if (!self.InputOk)
                return;
            if (ignoreInput)
            {
                self.IgnoreInput(this.lastTime);
                return;
            }
            this.startTime = startTime;
            if (this.isUsed == true)
            {
                foreach(var trigger in skillTriggers)
                    trigger.Release();
            }
            else
                this.isUsed = true;
            if (onSkillBegin != null)
                onSkillBegin(this);
            foreach (var trigger in skillTriggers)
                trigger.Execute(self);
            MainLoop.Instance.ExecuteLater(Reset,this.lastTime);
        }
    }

    
    /// <summary>
    /// 管理所有SkillInstance
    /// </summary>
    public class SkillSystem:Singleton<SkillSystem>
    {
        /// <summary>
        /// AbstractPerson通过技能名从这里获取技能索引
        /// </summary>
        public SerializableDictionary<string,SkillInstance> skillInstanceDic=new SerializableDictionary<string, SkillInstance>();
        
        public SkillSystem()
        {
            //所有触发器种类的注册
            SkillTriggerMgr.Instance.RegisterTriggerFactory("AnimationTrigger",
                SkillTriggerFactory<AnimationTrigger>.Instance);
            SkillTriggerMgr.Instance.RegisterTriggerFactory("InstantDamageTrigger",
                SkillTriggerFactory<InstantRayDamageTrigger>.Instance);
            SkillTriggerMgr.Instance.RegisterTriggerFactory("AudioTrigger",
                SkillTriggerFactory<AudioTrigger>.Instance);
            SkillTriggerMgr.Instance.RegisterTriggerFactory("DashTrigger",
                SkillTriggerFactory<DashTrigger>.Instance);
            SkillTriggerMgr.Instance.RegisterTriggerFactory("BulletTrigger",
                SkillTriggerFactory<BulletTrigger>.Instance);
            SkillTriggerMgr.Instance.RegisterTriggerFactory("Trigger2DTrigger",
                SkillTriggerFactory<Trigger2DTrigger>.Instance);
//            SkillTriggerMgr.Instance.RegisterTriggerFactory("LockFrameTrigger",
//                SkillTriggerFactory<LockFrameTrigger>.Instance);

            //读取文件，获取所有技能
            LoadSkillsFromFile(FilePath.SkillFilePath);
        }

        /// <summary>
        /// 文件读取技能
        /// </summary>
        /// <param name="path"></param>
        private void LoadSkillsFromFile(string path)
        {
            path = Application.streamingAssetsPath + "\\"+path;
            var sr=new StreamReader(path);
            
            bool bracket = false;
            SkillInstance skill = null;
            do 
            {
                string line = sr.ReadLine();
                if (line == null)
                    break;
 
                line = line.Trim();
                
                //注释写法   //注释
                if (line.StartsWith("//") || line == "")
                    continue;
                
                //解析文件
                if (line.StartsWith("skill"))//技能行
                {
                    //技能行写法  skill|"技能名"
                    var strs = line.Split('|');
                    skill = new SkillInstance(strs[1].Trim());
                    skillInstanceDic.Add(strs[1].Trim(),skill);
                }
                else if (line.StartsWith("{"))//开始大括号
                {
                    bracket = true;
                }
                else if (line.StartsWith("}"))//结束大括号
                {
                    bracket = false;
                }
                else//触发器块
                {
                    //触发器写法   触发器名|开始时间|持续时间|args（以，分隔）    如：AnimationTrigger|0|0.12|BeginAnimation
                    if (skill != null && bracket == true)
                    {

                        ISkillTrigger trigger = SkillTriggerMgr.Instance.CreateTrigger(line);//strs[0].Trim(), Convert.ToInt32(strs[2].Trim()),Convert.ToSingle(strs[1].Trim()),Convert.ToSingle(strs[3].Trim()),strs[4].Trim());
                        if (trigger != null)
                        {
                            skill.AddTrigger(trigger);
                        }
                    }
                } 
            } while (true);
        }

    }
    
}
