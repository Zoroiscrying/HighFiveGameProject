using Game.Const;
using Game.Control.BattleEffectSystem;
using Game.Control.SkillSystem;
using Game.Global;
using Game.Model;
using Game.Script;
using Game.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Game.Common;
using Game.MemorySystem;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.EventSystems;
using Object = UnityEngine.Object;

namespace Game.Control.PersonSystem
{
    /// <summary>
    /// 所有接受战斗效果的单位都要继承这个抽象类
    /// </summary>
    [Serializable]
    public abstract class AbstractPerson 
    {
        #region 根据GameObject获取AbstractPerson

        public static AbstractPerson GetInstance(GameObject go)
        {
            foreach (var g in instanceList)
                if (g.obj == go)
                    return g;
            return null;
        }

        public static T GetInstance<T>(GameObject go) where T : AbstractPerson
        {
            foreach (var g in instanceList)
                if (g.obj == go)
                    return g as T;
            return null;
        }

        protected static List<AbstractPerson> instanceList = new List<AbstractPerson>();

        #endregion

        #region 属性

        [HideInInspector]
        public GameObject obj;
        
        public string name { get; set; }              //名字
        public int Hp { get; set; }                   //血量
        public int MaxHp { get; set; }                //最大血量
        public int Attack { get; set; }               //攻击

        public bool IgnoreHitback { get; set; }       //忽略击退
        public bool IsConst { protected set; get; }     //是否可选中/无敌帧
        public float DefaultConstTime { protected set; get; } //硬直时间
        public bool InputOk { get; set; }             //接受技能输入
        public event Action OnThisUpdate;

        /// <summary>
        /// 攻击速度
        /// </summary>
        private float attackSpeed = 1;
        public float AttackSpeed
        {
            get
            {
                return this.attackSpeed;
            }
            set
            {
                if(value<=0)
                {
                    throw new Exception("攻速不能为零:"+this.name);
                }
                this.attackSpeed = value;
            }
        }

        /// <summary>
        /// 获取角色面对方向
        /// </summary>
        public int Dir
        {
            get
            {
                var v = this.obj.transform.localScale.x > 0 ? 1 : -1;
                return v;
            }
        }

        /// <summary>
        /// 获取物体位置
        /// </summary>
        public Vector3 Pos
        {
            get
            {
                if (this.obj == null)
                    return Const.Signal.defaultPos;
                return this.obj.transform.position;
            }
        }

        /// <summary>
        /// 获取缩放
        /// </summary>
        public float Scanler
        {
            get { return Mathf.Abs(this.obj.transform.localScale.x) / 3f; }
        }


        public Actor Actor
        {
            get { return this.obj.GetComponent<Actor>(); }
        }




        #endregion

        #region 技能

        private int maxRealSkillCount;
        public virtual int BaseSkillCount
        {
            get
            {
                return 0;
            }
        }
        public int MaxRealSkillCount
        {
            get
            {
                return maxRealSkillCount;
            }
        }
        public void IncreaseMaxSkillCount(int count)
        {
            maxRealSkillCount += count;
        }

        /// <summary>
        /// 存放所有可用的技能名
        /// </summary>
        protected List<string> allSkillNames;
        /// <summary>
        /// 存储技能实例
        /// </summary>
        protected List<SkillInstance> skills = new List<SkillInstance>();
        //protected SerializableDictionary<string, SkillInstance> skillDic = new SerializableDictionary<string, SkillInstance>();
        /// <summary>
        /// 存储动态添加的技能及其触发条件
        /// </summary>
        //protected List<UpdateTestPair> dis = new List<UpdateTestPair>();

        /// <summary>
        /// 释放技能
        /// </summary>
        /// <param name="skillName"></param>
        public void RunSkill(int index, bool ignoreInput = false)
        {
            if(!(skills.Count>=this.BaseSkillCount+this.MaxRealSkillCount))
            {
                throw new Exception(this.name + "没有这个技能索引：" +index);
            }
            this.skills[index].Execute(this, ignoreInput);
        }

        /// <summary>
        /// 添加技能
        /// </summary>
        /// <param name="skillName"></param>
        /// <param name="trigger"></param>
        public void AddSkill(string skillName, Func<bool> trigger, bool ignoreInput = false)
        {
            foreach(var s in skills)
            {
                if (s.name == skillName)
                {
                    Debug.LogWarning("重复添加技能");
                    return;
                }
            }
            if(skills.Count>=this.MaxRealSkillCount+this.BaseSkillCount)
            {
                Debug.LogWarning("无法添加更多技能");
                return;
            }
            this.allSkillNames.Add(skillName);
            var skill = SkillTriggerMgr.skillInstanceDic[skillName];
            this.skills.Add(skill);
            var pair = new UpdateTestPair(trigger, () => RunSkill(skills.Count-1, ignoreInput));
            MainLoop.Instance.AddUpdateTest(pair);
            //dis.Add(pair);
        }
        #endregion

        #region BattleEffect

        /// <summary>
        /// 攻击效果合集
        /// </summary>
        protected List<IBattleEffect> attackEffects = new List<IBattleEffect>();

        /// <summary>
        /// 这个事件允许动态添加主角攻击效果
        /// </summary>
        public event Action<AbstractPerson> OnAttackListRefresh;

        /// <summary>
        /// 获取攻击效果
        /// </summary>
        public virtual List<IBattleEffect> AttackEffect
        {
            get
            {
                attackEffects.Clear();
                if (OnAttackListRefresh != null)
                    OnAttackListRefresh(this);
                return attackEffects;
            }
        }

        /// <summary>
        /// 当前Buff列表
        /// </summary>
        public List<IBattleEffect> bfList = new List<IBattleEffect>();

        /// <summary>
        /// 接受战斗效果
        /// </summary>
        /// <param name="ef"></param>
        public virtual void TakeBattleEffect(IBattleEffect ef)
        {
            this.bfList.Add(ef);
            ef.Execute(this);
        }
        public virtual void TakeBattleEffect(List<IBattleEffect> beList)
        {
            foreach (var be in beList)
                this.TakeBattleEffect(be);
        }


        #endregion


        #region 构造及初始化

        public AbstractPerson()
        {
        }

        public AbstractPerson(string path, string prefabPath, Vector3 pos, List<string> skillTypes = null, Transform parent = null)
        {
            //初始化默认属性
            this.name = name;
            this.InputOk = true;
            this.IsConst = false;
            this.IgnoreHitback = false;
            this.DefaultConstTime = 0;
            
            this.obj = MemoryMgr.Instantiate(path, pos, Quaternion.identity, parent);


            allSkillNames = skillTypes;
            //初始化技能
            if (skillTypes != null)
                foreach (var str in skillTypes)
                    //skillDic.Add(str, SkillTriggerMgr.skillInstanceDic[str]);
                    skills.Add(SkillTriggerMgr.skillInstanceDic[str]);

            if(skillTypes!=null)
                this.maxRealSkillCount = skillTypes.Count-this.BaseSkillCount;
            else
            {
                this.maxRealSkillCount = 0;
            }

            //添加基本攻击效果
            this.OnAttackListRefresh += AddBaseAttackEffects;
            //添加事件监听
            OnAddListener();

            //开始每帧Update
            MainLoop.Instance.AddUpdateFunc(Update);

            //记录每个实例
            instanceList.Add(this);
        }

        /// <summary>
        /// 添加基础攻击效果
        /// </summary>
        /// <param name="ap"></param>
        protected abstract void AddBaseAttackEffects(AbstractPerson self);

        #endregion

        #region 战斗

        public virtual void  OnCauseDamage(int damage)
        {
            
        }

        public virtual void OnTakeDamage(int damage)
        {
            
        }

        /// <summary>
        /// 变为硬直状态
        /// </summary>
        public void ToConst(float time)
        {
            Assert.IsTrue(!IsConst);
            IsConst = !IsConst;
            //            Debug.Log(this.obj.GetInstanceID() + "硬直 "+this.IsConst);
            MainLoop.Instance.ExecuteLater(_Reset, time);
            //硬直动画
        }

        /// <summary>
        /// 不接受输入
        /// </summary>
        /// <param name="time"></param>
        public void IgnoreInput(float time)
        {
            this.InputOk = false;
            MainLoop.Instance.ExecuteLater(_IgnoreInput, time);
        }

        /// <summary>
        /// 死亡效果
        /// </summary>
        /// <returns></returns>
        public virtual void DestoryThis()
        {
           
            OnRemoveListener();
            MainLoop.Instance.RemoveUpdateFunc(Update);
            instanceList.Remove(this);
            Object.Destroy(this.obj);
        }
        #endregion

        #region Update及CEventCenter

        /// <summary>
        /// 每帧调用
        /// </summary>
        protected virtual void Update()
        {
            if (OnThisUpdate != null)
                OnThisUpdate();
        }

        /// <summary>
        /// 添加事件监听-
        /// </summary>
        public virtual void OnAddListener()
        {
            CEventCenter.AddListener<int>(Message.M_BloodChange(this.obj), OnBloodChanged);
            CEventCenter.AddListener(Message.M_Destory(this.obj), DestoryThis);
        }

        /// <summary>
        /// 移除事件监听
        /// </summary>
        public virtual void OnRemoveListener()
        {
            CEventCenter.RemoveListener<int>(Message.M_BloodChange(this.obj), OnBloodChanged);
            CEventCenter.RemoveListener(Message.M_Destory(this.obj), DestoryThis);
        }

        /////////////    事件处理函数     //////////////

        protected virtual void OnBloodChanged(int change)
        {
            var damage = -change;
            //这里只处理数值，动画Ui什么的另加监听者
            if (this.IsConst)
                return;
            this.Hp -= damage;
            if (this.Hp < 0)
            {
                //死亡
                this.Hp = 0;
                CEventCenter.BroadMessage(Message.M_Destory(this.obj));
                CEventCenter.BroadMessage<PointerEventData>(Message.M_Destory(this.obj), null);
            }
            else
            {
                //硬直
                this.ToConst(this.DefaultConstTime);
            }
        }

        #endregion

        #region 内部调用

        private void _IgnoreInput()
        {
            this.InputOk = true;
        }

        private void _Reset()
        {
            //            Debug.Log(this.obj.GetInstanceID() + "恢复");
            Assert.IsTrue(IsConst);
            IsConst = !IsConst;
        }

        #endregion
    }
}
