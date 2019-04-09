using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.EventSystems;
using Game.Global;
using Game.Const;
using Game.Modal;
using Game.View;
using Game.Script;
using Game.Serialization;
using Object = UnityEngine.Object;
using zoroiscrying;

namespace Game.Control
{
    ////////////////////////////////     战斗单位基类       /////////////////////////////

    /// <summary>
    /// 所有接受战斗效果的单位都要继承这个抽象类
    /// </summary>
    [Serializable]
    public abstract class AbstractPerson : PersonControl
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

        public string name { get; set; }              //名字
        public int Hp { get; set; }                   //血量
        public int MaxHp { get; set; }                //最大血量
        public int Attack { get; set; }               //攻击
        
        public bool IgnoreHitback { get; set; }       //忽略击退
        public bool IsConst { protected set; get; }     //是否可选中
        public float DefaultConstTime { protected set; get; } //硬直时间
        public bool InputOk { get; set; }             //接受技能输入
        protected List<string> allSkillNames;
        private event Action OnThisUpdate;
        
        /// <summary>
        /// 获取角色面对方向
        /// </summary>
        public int Dir
        {
            get
            {
                var v=this.obj.transform.localScale.x > 0 ? 1 : -1;
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

        # region 技能
        
        public SerializableDictionary<string,SkillInstance> skillDic=new SerializableDictionary<string, SkillInstance>();

        /// <summary>
        /// 释放技能
        /// </summary>
        /// <param name="skillName"></param>
        public void RunSkill(string skillName,bool ignoreInput=false)
        {
            this.skillDic[skillName].Execute(this,ignoreInput);
        }
        /// <summary>
        /// 添加技能
        /// </summary>
        /// <param name="skillName"></param>
        /// <param name="trigger"></param>
        public void AddSkill(string skillName,Func<bool> trigger)
        {
            this.allSkillNames.Add(skillName);
            this.skillDic.Add(skillName,SkillTriggerMgr.skillInstanceDic[skillName]);
            dis.Add(trigger,skillName);
        }
        #endregion
        
        #region BattleEffect
        
        /// <summary>
        /// 攻击效果合集
        /// </summary>
        protected List<IBattleEffect> attackEffects=new List<IBattleEffect>();

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
        public List<IBattleEffect> bfList=new List<IBattleEffect>();
        
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
            foreach(var be in beList)
                this.TakeBattleEffect(be);
        }

       
        #endregion

        #region private
        
        SerializableDictionary<Func<bool>,string> dis=new SerializableDictionary<Func<bool>, string>();
        
        #endregion
        
        #region 构造及初始化

        public AbstractPerson()
        {
        }

        public AbstractPerson(string name, string prefabPath, Vector3 pos, List<string> skillTypes=null,Transform parent=null) : base(prefabPath)
        {
            //初始化默认属性
            this.name = name;
            this.InputOk = true;
            this.IsConst = false;
            this.IgnoreHitback = false;
            this.DefaultConstTime = 0;
            this.obj = Object.Instantiate(this.obj, pos, Quaternion.identity, parent);
            
            
            //初始化外显属性
            if(this is Player)
                Init(PersonData.Instance.rankArgs[0]);
            else
                Init(PersonData.Instance.enenyArgs[this.name]);


            allSkillNames = skillTypes;
            //初始化技能
            if(skillTypes!=null)
                foreach (var str in skillTypes)
                    skillDic.Add(str,SkillTriggerMgr.skillInstanceDic[str]);
            
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
        /// 初始化属性
        /// 这里通过从字符串args中获取信息初始化除了名字以外其他属性
        /// </summary>
        /// <param name="args"></param>
        protected virtual void Init(string args)
        {
            var strs = args.Split('|');
            Assert.IsTrue(strs.Length>=2);
            this.MaxHp = Convert.ToInt32(strs[0].Trim());
            this.Attack = Convert.ToInt32(strs[1].Trim());
            this.Hp = this.MaxHp;
        }

        
        /// <summary>
        /// 添加基础攻击效果
        /// </summary>
        /// <param name="ap"></param>
        protected abstract void AddBaseAttackEffects(AbstractPerson self);

        #endregion
        
        #region 战斗

        
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
            if(!(this is Player))
                CEventCenter.BroadMessage(Message.M_ExpChange,50*GlobalVar.Player.rank);
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
            if (OnThisUpdate!=null)
                OnThisUpdate();
            foreach (var w in dis)
            {
                if (w.Key())
                    this.RunSkill(w.Value);
            }
        }

        /// <summary>
        /// 添加事件监听-
        /// </summary>
        public virtual void OnAddListener()
        {
            CEventCenter.AddListener<int>(Message.M_BloodChange(this.obj),OnBloodChanged);
            CEventCenter.AddListener(Message.M_Destory(this.obj),DestoryThis);
        }

        /// <summary>
        /// 移除事件监听
        /// </summary>
        public virtual void OnRemoveListener()
        {
            CEventCenter.RemoveListener<int>(Message.M_BloodChange(this.obj),OnBloodChanged);
            CEventCenter.RemoveListener(Message.M_Destory(this.obj),DestoryThis);
        }
        
        /////////////    事件处理函数     //////////////

        protected virtual void OnBloodChanged(int change)
        {
            var damage = -change;
            //这里只处理数值，动画Ui什么的另加监听者
            if (this.IsConst)
                return;
            this.Hp -=damage;
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


    
    /// <summary>
    /// 玩家类
    /// </summary>
    [Serializable]
    public class Player : AbstractPerson,IXmlSerializable
    {

    
        #region 玩家专有外显属性

        [HideInInspector]
        public int attackMultipulier=1;  //攻击力翻倍
        [HideInInspector]
        public int attackAdder=0;        //攻击力叠加
        
        public float hitBackSpeed=0.08f;    //击退
        public int rank;      //灵力等级
        public int MaxExp;    //最大灵力上限
        public int Exp;       //当前灵力
        public int Maxdrag;   //最大药引上限
        public int drag;      //当前药引


        #endregion
        
        #region 连击

        
        public float airXMove;
        private int comboNum = 0;
        private float timer = 0;
        
        public List<float> beginComboTest=new List<float>();//0~1之间
        public List<float> durTimes=new List<float>();
        public List<float> canMoveTime=new List<float>();        
        public List<string> skillNames=new List<string>();
        public List<bool> ignoreInput=new List<bool>();
        #endregion
        
        #region 背包

        public Backpack backpack=new Backpack();
        
        #endregion
        
        #region 灵器

        public int MaxSpiritNum=1;

        private SerializableDictionary<string, AbstractSpiritItem> spiritDic =
            new SerializableDictionary<string, AbstractSpiritItem>();


        public void AddSpirit(string spiritName)
        {
            if (spiritDic.Count >= this.MaxSpiritNum)
            {
                Debug.Log("无法承载更多灵器");
                return;
            }
            var spirit = AbstractSpiritItem.GetInstance(spiritName);
            spiritDic.Add(spiritName,spirit);
            spirit.OnEnable();
        }

        public void RemoveSpirit(string spiritName)
        {
            spiritDic.Remove(spiritName);
            AbstractSpiritItem.GetInstance(spiritName).OnDisable();
        }
        
        #endregion
        
        #region 消耗灵力的强化系统


        /// 外界响应灵力释放有两种方式：
        ///     一种是判断Player.IsSuper属性
        ///     一种是响应Message.InitSuper和Message.ExitSuper消息
  
        public static bool isSuper { get; private set; }
        public static float superTime = 3;
        private void TrySuper()
        {
            //if (Convert.ToSingle(this.Exp) / Convert.ToSingle(this.MaxExp) >= 1 / 3.0f&&!isSuper)
            //{
                //进入强化状态
                //可能要UI，动画，特效，移动，各个领域配合
                CEventCenter.BroadMessage(Message.M_InitSuper);
                MainLoop.Instance.ExecuteLater(() => { CEventCenter.BroadMessage(Message.M_ExitSuper); }, superTime);
            //}
        }
        
        #endregion

        #region BattleEffect
        
        public override void TakeBattleEffect(List<IBattleEffect> beList)
        {
            base.TakeBattleEffect(beList);
            new AudioEffect("Fuck").Execute(this);
            new Shine(this.DefaultConstTime,0.1f, Color.blue).Execute(this);
            new ShakeScreen(0.05f,Time.deltaTime).Execute(this);
        }
        
        
        protected override void AddBaseAttackEffects(AbstractPerson self)
        {
            var player = self as Player;
            Assert.IsTrue(player!=null);
            player.attackEffects.Add(new InstantDamage(GameMath.Damage(player.Attack,player.attackMultipulier,player.attackAdder),player.Dir));
            player.attackEffects.Add(new Hitback(new Vector2(player.Dir*player.hitBackSpeed,0)));
        }
        
        #endregion

        #region 战斗
        public override void DestoryThis()
        {
            base.DestoryThis();
            Debug.Log("主角死了");
        }
        
        #endregion

        #region Private

        private MainCharacter mainc;
        private CharacterController2D cc;

        #endregion
        
        #region 构造和初始化        


        public Player()
        {
            
        }

        public Player(string name, string prefabPath, Vector3 pos, List<string> skillTypes,Transform parent=null) : base(name, prefabPath, pos, skillTypes,parent)
        {
            
            GlobalVar.Player = this;
            Debug.Log("主角诞生啦");
            this.DefaultConstTime = 1.0f;
            mainc = this.obj.GetComponent<MainCharacter>();
            Assert.IsTrue(mainc!=null);
            cc = this.obj.GetComponent<CharacterController2D>();
            Assert.IsTrue(cc != null);
        }

        protected override void Init(string args)
        {
            base.Init(args);
            this.DefaultConstTime = 0.7f;
            OnLevelUp(1);
        }


        #endregion

        #region 更新和事件
        
        protected override void Update()
        {
//            Debug.Log("玩家在" + Pos);
            
            if (Input.GetKeyDown(KeyCode.L))
            {
                skillDic["L_Skill"].Execute(this,true);
            }

            if (Input.GetKeyDown(KeyCode.H))
            {
                skillDic["H_Skill"].Execute(this);
            }

            if (Input.GetKeyDown(KeyCode.RightAlt))
            {
                TrySuper();
                
            }
            
            #region 三连击
             
            SkillInstance lastSkill = null;
            var e=skillNames.GetEnumerator();
            for (var i = 0; i < this.comboNum; i++)
                e.MoveNext();

            if (this.comboNum >0)
            {
                lastSkill = skillDic[e.Current];
                Assert.IsTrue(this.comboNum>0&&this.comboNum<4);

                if (this.timer - lastSkill.startTime > canMoveTime[this.comboNum - 1])
                    mainc._inControl = true;
                
                 if  (this.timer - lastSkill.startTime >
                    lastSkill.LastTime * this.beginComboTest[this.comboNum-1] + durTimes[this.comboNum - 1])
                {
//                    Debug.Log("归零");
                    this.comboNum = 0;
                    MainLoop.Instance.RemoveUpdateFunc(_DeUpdate);
                    //恢复人物控制
                    mainc._inControl = true;
                }
            }

            if (Input.GetKeyDown(KeyCode.J))
            {
                if (!cc.isGrounded)
                {
                    mainc._playerVelocityY = 0;
                    this.TakeBattleEffect(new Hitback(new Vector2(this.Dir*Mathf.Abs(this.airXMove), 0)));
                }
                mainc._inControl = false;
            
//            Debug.Log("按下J");
                
                e.MoveNext();
                var thisSkill = skillDic[e.Current];
                    
                
                             
                if (this.comboNum == 0) //初次攻击
                {
                    this.timer = 0;
                    //开始增加时间
//                    Debug.Log("初次攻击");
                    MainLoop.Instance.AddUpdateFunc(_DeUpdate);
                    thisSkill.Execute(this,ignoreInput[0],this.timer);
                    this.comboNum++;
                }
                
                else if (
                    this.timer - lastSkill.startTime > lastSkill.LastTime * this.beginComboTest[this.comboNum-1] &&
                    this.timer - lastSkill.startTime <
                    lastSkill.LastTime * this.beginComboTest[this.comboNum-1] + durTimes[this.comboNum - 1] &&
                    this.comboNum<skillNames.Count)
                {
//                    Debug.Log(this.comboNum+1+"次攻击");
                    thisSkill.Execute(this, ignoreInput[this.comboNum-1], this.timer);
                    this.comboNum++;
                }
            }
            #endregion
            
        }
        
        
        //////////////////////////    消息处理     /////////////////////////////
        /// 这里只处理数值变化

        public override void OnAddListener()
        {
            base.OnAddListener();
            CEventCenter.AddListener(Message.M_InitSuper, InitSuper);
            CEventCenter.AddListener(Message.M_ExitSuper, ExitSuper);
            CEventCenter.AddListener<int>(Message.M_ExpChange,OnExpChanged);
            CEventCenter.AddListener<int>(Message.M_LevelUp,OnLevelUp);
            CEventCenter.AddListener<int>(Message.M_DragChange,OnDragChanged);
        }

        public override void OnRemoveListener()
        {
            base.OnRemoveListener();
            CEventCenter.RemoveListener(Message.M_InitSuper, InitSuper);
            CEventCenter.RemoveListener(Message.M_ExitSuper, ExitSuper);
            CEventCenter.RemoveListener<int>(Message.M_ExpChange,OnExpChanged);
            CEventCenter.RemoveListener<int>(Message.M_LevelUp,OnLevelUp);
            CEventCenter.RemoveListener<int>(Message.M_DragChange,OnDragChanged);
        }

        void InitSuper()
        {
            isSuper = true;
        }

        void ExitSuper()
        {
            isSuper = false;
        }
        

        /// <summary>
        /// 药引改变
        /// </summary>
        /// <param name="change"></param>
        void OnDragChanged(int change)
        {
            this.drag += change;
            if (this.drag > this.Maxdrag)
                this.drag = this.Maxdrag;
        }
        
        /// <summary>
        /// 灵力改变
        /// </summary>
        /// <param name="change"></param>
        void OnExpChanged(int change)
        {
            if (this.Exp + change >= this.MaxExp)
            {
                CEventCenter.BroadMessage(Message.M_LevelUp,this.rank+1);
            }
            else
            {
                this.Exp += change;
            }
        }
        
        /// <summary>
        /// 升级
        /// </summary>
        public void OnLevelUp(int newRank)
        {
//            Debug.Log("人物升级："+newRank);
            this.Exp = 0;
            this.rank = newRank;
            var strs = PersonData.Instance.rankArgs[this.rank-1].Split('|');
            Assert.IsTrue(strs.Length>=4);
            this.Hp=this.MaxHp = Convert.ToInt32(strs[0].Trim());
            this.Attack = Convert.ToInt32(strs[1].Trim());
            this.MaxExp = Convert.ToInt32(strs[2].Trim());
            this.Maxdrag = Convert.ToInt32(strs[3].Trim());
        }

        #endregion


        #region 内部调用
        void _DeUpdate()
        {
            this.timer += Time.deltaTime;
        }
        #endregion

        #region IXmlSerilize
        
        public XmlSchema GetSchema()
        {
            return null;
        }

        public void ReadXml(XmlReader reader)
        {
            var floatSer=new XmlSerializer(typeof(float));
            var intSer=new XmlSerializer(typeof(int));
            var strSer=new XmlSerializer(typeof(string));
            var vector3Ser = new XmlSerializer(typeof(Vector3));
            var strListSer=new XmlSerializer(typeof(List<string>));

            reader.Read();
            reader.ReadStartElement("name");
            this.name = (string) strSer.Deserialize(reader);
            reader.ReadEndElement();

            reader.ReadStartElement("Pos");
            var pos = (Vector3) vector3Ser.Deserialize(reader);
            this.obj = Resources.Load<GameObject>(GameData.PlayerPath);
            this.obj = Object.Instantiate(this.obj, pos, Quaternion.identity);
            reader.ReadEndElement();

            if (this.obj == null)
                Debug.LogError("主角为空了，干");
            
            reader.ReadStartElement("Attack");
            this.Attack = (int) intSer.Deserialize(reader);
            reader.ReadEndElement();

            reader.ReadStartElement("Skills");
            this.allSkillNames = (List<string>) strListSer.Deserialize(reader);
            foreach(var skill in allSkillNames)
                this.skillDic.Add(skill,SkillTriggerMgr.skillInstanceDic[skill]);
            reader.ReadEndElement();
            
            attackEffects=new List<IBattleEffect>();
            this.InputOk = true;
            this.IsConst = false;
            this.IgnoreHitback = false;

            this.comboNum = 0;
            this.timer = 0;
        
            this.beginComboTest=new List<float>();//0~1之间
            this.durTimes=new List<float>();
            this.canMoveTime=new List<float>();        
            this.skillNames=new List<string>();
            this.ignoreInput=new List<bool>();
            this.backpack=new Backpack();
            
            GlobalVar.Player = this;
            mainc = this.obj.GetComponent<MainCharacter>();
            Assert.IsTrue(mainc!=null);
            cc = this.obj.GetComponent<CharacterController2D>();
            Assert.IsTrue(cc != null);
            this.DefaultConstTime = 0.7f;
            this.attackMultipulier=1;  //攻击力翻倍
            this.attackAdder=0;        //攻击力叠加

            reader.ReadStartElement("HitbackSpeed");
            this.hitBackSpeed = (float) floatSer.Deserialize(reader);
            reader.ReadEndElement();
            
            reader.ReadStartElement("Rank");
            this.rank = (int) intSer.Deserialize(reader);
            reader.ReadEndElement();

            //添加基本攻击效果
            this.OnAttackListRefresh += AddBaseAttackEffects;
            
            //添加事件监听
            OnAddListener();
            
            //开始每帧Update
            MainLoop.Instance.AddUpdateFunc(Update);
            
            //记录每个实例
            instanceList.Add(this);
            reader.ReadEndElement();
        }

        public void WriteXml(XmlWriter writer)
        {
//            Debug.Log("这真的调用了么");
            var floatSer=new XmlSerializer(typeof(float));
            var intSer=new XmlSerializer(typeof(int));
            var strSer=new XmlSerializer(typeof(string));
            var vector3Ser = new XmlSerializer(typeof(Vector3));
            var strListSer=new XmlSerializer(typeof(List<string>));
//            var boolSer = new XmlSerializer(typeof(bool));
//            var boolListSer=new XmlSerializer(typeof(List<bool>));
//            var floatListSer=new XmlSerializer(typeof(List<float>));
//            var skillDicSer=new XmlSerializer(typeof(SerializableDictionary<string,SkillInstance>));
            
            writer.WriteStartElement("name");
            strSer.Serialize(writer, this.name);
            writer.WriteEndElement();

            writer.WriteStartElement("Pos");
            vector3Ser.Serialize(writer, this.Pos);
            writer.WriteEndElement();


            writer.WriteStartElement("Attack");
            intSer.Serialize(writer,this.Attack);
            writer.WriteEndElement();

            writer.WriteStartElement("Skills");
            strListSer.Serialize(writer,this.allSkillNames);
            writer.WriteEndElement();

            writer.WriteStartElement("HitbackSpeed");
            floatSer.Serialize(writer,this.hitBackSpeed);
            writer.WriteEndElement();

            writer.WriteStartElement("Rank");
            intSer.Serialize(writer, this.rank);
            writer.WriteEndElement();

//            writer.WriteStartElement("Skills");
//            skillDicSer.Serialize(writer,this.skillDic);


        }
        
        #endregion
    }


    /// <summary>
    /// 测试草人
    /// </summary>
    public class TestPerson : AbstractPerson
    {
        #region Private
        
        private float shineLastTime=1;
        private float shineDurTime=0.1f;
        private float hitback = 0.05f;
        
        #endregion
        
        public TestPerson(string name, string prefabPath, Vector3 pos, List<string> skillTypes,Transform parent=null) : base(name, prefabPath, pos, skillTypes,parent)
        {
            new BloodBar(this);
        }

        #region BattleEffect
        public override void TakeBattleEffect(List<IBattleEffect> beList)
        {
            base.TakeBattleEffect(beList);
            new AudioEffect("Fuck").Execute(this);
            new Shine(this.shineLastTime,this.shineDurTime, Color.red).Execute(this);
            new ShakeScreen(0.02f,Time.deltaTime).Execute(this);
        }

        protected override void AddBaseAttackEffects(AbstractPerson self)
        {
            TestPerson tp=self as TestPerson;
            Assert.IsTrue(tp!=null);
            tp.attackEffects.Add(new InstantDamage(this.Attack, this.Dir));
            tp.attackEffects.Add(new Hitback(new Vector2(this.hitback,0)));
        }
        #endregion
    }

    
    /// <summary>
    /// 战斗单位数值系统
    /// </summary>
    public class PersonData : Singleton<PersonData>
    {
        
        public List<string> rankArgs = new List<string>();
        public SerializableDictionary<string, string> enenyArgs=new SerializableDictionary<string, string>();

        public PersonData()
        {
            LoadPersonArgsFromFile(FilePath.PersonArgsFilePath);
        }

        private void LoadPersonArgsFromFile(string path)
        {
            bool rank = false;
            bool enemy = false;
            bool backet = false;
            path = Application.streamingAssetsPath + "\\"+path;
            var sr = new StreamReader(path);
            do
            {
                string line = sr.ReadLine();
                if (line == null)
                    break;

                line = line.Trim();

//                Debug.Log(line);
                //注释写法   //注释
                if (line.StartsWith("//") || line == "")
                    continue;

                if (line.StartsWith("Rank"))
                {
                    rank = true;
//                    Debug.Log("Enemy: " + enemy + " Rank: " + rank + " Backet: " + backet);

                    continue;
                }

                if (line.StartsWith("{"))
                {
                    backet = true;
//                    Debug.Log("Enemy: " + enemy + " Rank: " + rank + " Backet: " + backet);

                    continue;
                }
                
                if (line.StartsWith("Enemy"))
                {
                    enemy = true;
//                    Debug.Log("Enemy: " + enemy + " Rank: " + rank + " Backet: " + backet);

                    continue;
                }
                if (line.EndsWith("}"))
                {
                    backet = false;
                    if (enemy)
                        enemy = false;
                    if (rank)
                        rank = false;
//                    Debug.Log("Enemy: " + enemy + " Rank: " + rank + " Backet: " + backet);

                    continue;
                }

//                Debug.Log("Enemy: " + enemy + " Rank: " + rank + " Backet: " + backet);
                if (backet)
                {
                    if (rank)
                    {
                        rankArgs.Add(line);
                    }

                    if (enemy)
                    {
                        var strs = line.Split('|');
                        enenyArgs.Add(strs[0].Trim(), string.Join("|",strs,1,strs.Length-1));
                    }
                }
                
                
            } while (true);
        }
    }
}
