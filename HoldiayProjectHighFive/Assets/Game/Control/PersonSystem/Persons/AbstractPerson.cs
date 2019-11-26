using Game.Const;
using Game.Control.SkillSystem;
using System;
using System.Collections.Generic;
using Game.Control.EffectSystem;
using ReadyGamerOne.Common;
using ReadyGamerOne.MemorySystem;
using ReadyGamerOne.Script;
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
    public abstract class AbstractPerson:IEffector<AbstractPerson>
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

        #region Fields
        
        protected BaseCharacterInfo characterInfoInfo;
        
        [HideInInspector]
        public GameObject obj;
        
        /// <summary>
        /// 当前血量
        /// </summary>
        private int hp;
        /// <summary>
        /// 是否无敌
        /// </summary>
        private bool isConst=false;
        /// <summary>
        /// 攻击叠加
        /// </summary>
        public float attack_adder = 0f;
        /// <summary>
        /// 攻击倍率
        /// </summary>
        public float attack_scaler = 1f;
        
        /// <summary>
        /// 角色名字
        /// </summary>
        public string CharacterName => characterInfoInfo.characterName;          //名字
        
        
        #endregion        
        
//        #region Abstract_Properties
//        
//        
//        /// <summary>
//        /// 最大生命值
//        /// </summary>
//        public abstract int MaxHp { get; set; }        
//
//        /// <summary>
//        /// 攻击力
//        /// </summary>
//        public abstract int BaseAttack { get; set; }
//
//        /// <summary>
//        /// 是否忽略击退
//        /// </summary>
//        public abstract bool IgnoreHitback { get; set; }
//
//        /// <summary>
//        /// 硬直事件
//        /// </summary>
//        public abstract float DefaultConstTime  { get; set; }        
//        
//        /// <summary>
//        /// 攻击速度
//        /// </summary>
//        public abstract float AttackSpeed { get; set; }
//        
//
//        #endregion

        #region Public_Properties

        #region 依赖资源的属性

        /// <summary>
        /// 最大生命值
        /// </summary>
        public int MaxHp
        {
            get { return characterInfoInfo.maxHp; }
            set { characterInfoInfo.maxHp = value; }
        }

        /// <summary>
        /// 攻击力
        /// </summary>
        public int BaseAttack
        {
            get { return characterInfoInfo.baseAttack; }
            set { characterInfoInfo.baseAttack = value; }
        }

        /// <summary>
        /// 是否忽略击退
        /// </summary>
        public bool IgnoreHitback
        {
            get { return characterInfoInfo.IgnoreHitback; }
            set { characterInfoInfo.IgnoreHitback = value; }
        }
        
        /// <summary>
        /// 硬直事件
        /// </summary>
        public float DefaultConstTime
        {
            get { return characterInfoInfo.DefaultConstTime; }
            set { characterInfoInfo.DefaultConstTime = value; }
        }
        
        /// <summary>
        /// 攻击速度
        /// </summary>
        public float AttackSpeed
        {
            get { return characterInfoInfo.attackSpeed; }
            set
            {
                if (value <= 0)
                {
                    throw new Exception("攻速不能为零:" + this.CharacterName);
                }

                characterInfoInfo.attackSpeed = value;
            }
        }    
        
        /// <summary>
        /// 玩家被击退速度
        /// </summary>
        public Vector2 HitBackSpeed
        {
            get { return characterInfoInfo.hitBackSpeed; }
            set { characterInfoInfo.hitBackSpeed = value; }
        }
        
        
        

        #endregion
        

        #region 本地属性
        /// <summary>
        /// 血量
        /// </summary>
        public int Hp
        {
            get { return hp; }
            set { hp = value; }
        }     
        
        /// <summary>
        /// 是否进入无敌帧
        /// </summary>
        public bool IsConst
        {
            protected set { isConst = value; }
            get { return isConst; }
        }  
        
        /// <summary>
        /// 获取角色面对方向
        /// </summary>
        public int Dir
        {
            get
            {
                if (obj == null)
                    return 0;
                var v = this.obj.transform.localScale.x > 0 ? 1 : -1;
                return v;
            }
            set
            {
                var dir = value > 0 ? 1 : -1;
                var s = obj.transform.localScale;
                obj.transform.localScale = new Vector3(dir * Mathf.Abs(s.x), s.y, s.z);
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

        /// <summary>
        /// 玩家
        /// </summary>
        public Actor Actor
        {
            get { return this.obj.GetComponent<Actor>(); }
        }
        
        #endregion
 

        #endregion

        #region Public_Functions


        /// <summary>
        /// 看向某个物体，【当然，只是左右】
        /// </summary>
        /// <param name="target"></param>
        public void LookAt(Transform target)
        {
            var dir = target.transform.position.x - obj.transform.position.x > 0 ? 1 : -1;
            var scale = obj.transform.localScale;
            obj.transform.localScale = new Vector3(dir* Mathf.Abs(scale.x), scale.y, scale.z);
        }
        
        /// <summary>
        /// 执行技能
        /// </summary>
        /// <param name="skillInfoAsset"></param>
        public void RunSkill(SkillInfoAsset skillInfoAsset)
        {
            skillInfoAsset.RunSkill(this);
        }
        

        #region 构造及初始化

        public AbstractPerson()
        {
        }

        public AbstractPerson(BaseCharacterInfo characterInfo,Vector3 pos, Transform parent = null)
        {
            this.characterInfoInfo = characterInfo;
            
            this.obj = MemoryMgr.InstantiateGameObject(characterInfo.prefabPath.Path, pos, Quaternion.identity, parent);
            this.Hp = characterInfo.maxHp;

            //添加事件监听
            OnAddListener();

            //开始每帧Update
            MainLoop.AddUpdateFunc(Update);

            //记录每个实例
            instanceList.Add(this);
        }

        #endregion

        #region 战斗

        /// <summary>
        /// 播放受击特效
        /// </summary>
        /// <param name="ditascher"></param>
        public void PlayAcceptEffects(IEffector<AbstractPerson> ditascher)
        {
            if (ditascher != null && ditascher.HitEffects != null)
            {
                ditascher.HitEffects.Play(ditascher, this);
            }

            if (AcceptEffects != null)
            {
                AcceptEffects.Play(null,ditascher);
            }
        }

        /// <summary>
        /// 播放攻击特效
        /// </summary>
        /// <param name="effectInfoAsset"></param>
        public void PlayAttackEffects(EffectInfoAsset effectInfoAsset)
        {
            effectInfoAsset?.Play(this,null);
        }

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
        /// 死亡效果
        /// </summary>
        /// <returns></returns>
        public virtual void DestoryThis()
        {
           
            OnRemoveListener();
            MainLoop.RemoveUpdateFunc(Update);
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

        #endregion


        #region 内部调用

        private void _Reset()
        {
            //            Debug.Log(this.obj.GetInstanceID() + "恢复");
            Assert.IsTrue(IsConst);
            IsConst = !IsConst;
        }

        #endregion


        #region IEffector<T>
        public EffectInfoAsset AttackEffects => characterInfoInfo.attackEffects;
        public EffectInfoAsset HitEffects => characterInfoInfo.hitEffects;
        public EffectInfoAsset AcceptEffects => characterInfoInfo.acceptEffects;
        public AbstractPerson EffectPlayer => this;

        #endregion


    }
}
