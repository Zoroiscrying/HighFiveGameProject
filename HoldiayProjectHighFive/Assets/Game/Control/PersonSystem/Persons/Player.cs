using Game.Const;
using Game.Control.BattleEffectSystem;
using Game.Math;
using Game.Model.SpiritItemSystem;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using zoroiscrying;
using System.IO;
using Game.Model.ItemSystem;
using Game.Model.RankSystem;
using ReadyGamerOne.Common;
using ReadyGamerOne.Data;
using ReadyGamerOne.Global;
using ReadyGamerOne.Script;
using ReadyGamerOne.View.AssetUi;
using ReadyGamerOne.View.PanelSystem;


namespace Game.Control.PersonSystem
{
    /// <summary>
    /// 玩家类
    /// </summary>
    [Serializable]
    public class Player : AbstractPerson
    {
        #region 初始化玩家

        public static void InitPlayer()
        {
            if (File.Exists(DefaultData.PlayerDataFilePath))
            {
                Debug.Log("player comes from files");
                GlobalVar.G_Player = XmlManager.LoadData<Player>(DefaultData.PlayerDataFilePath);
            }

        }

        public static void InitPlayer(Vector3 pos)
        {
            if (File.Exists(DefaultData.PlayerDataFilePath))
            {
                Debug.Log("player comes from files");
                GlobalVar.G_Player = XmlManager.LoadData<Player>(DefaultData.PlayerDataFilePath);
            }
        }

        #endregion

        #region 背包

        /// <summary>
        /// 保存存储物品信息的内部类
        /// </summary>
        public class ItemData
        {
            public int count;
            public int itemId;

            public ItemData(int itemid, int count)
            {
                this.count = count;
                itemId = itemid;
            }
        }

        /// <summary>
        /// 添加物品
        /// </summary>
        /// <param CharacterName="itemId"></param>
        /// <param CharacterName="count"></param>
        public void AddItem(int itemId, int count)
        {
            var itemList = (characterInfoInfo as PlayerInfo).itemList;
            foreach (var i in itemList)
            {
                if (i.itemId == itemId)
                {
                    i.count += count;
                    return;
                }
            }

            itemList.Add(new ItemData(itemId, count));
        }

        /// <summary>
        /// 移出物品
        /// </summary>
        /// <param CharacterName="itemId"></param>
        /// <param CharacterName="count"></param>
        public void RemoveItem(int itemId, int count)
        {
            var itemList = (characterInfoInfo as PlayerInfo).itemList;
            foreach (var i in itemList)
            {
                if (i.itemId == itemId && i.count >= count)
                {
                    i.count -= count;
                    return;
                }
            }

            Debug.LogWarning("没有那么多这种东西：" + itemId);
        }

        /// <summary>
        /// 获取指定物品数量
        /// </summary>
        /// <param CharacterName="id"></param>
        /// <returns></returns>
        public int HaveItem(int id)
        {
            var itemList = (characterInfoInfo as PlayerInfo).itemList;
            foreach (var item in itemList)
                if (item.itemId == id)
                    return item.count;
            return 0;
        }

        #endregion


        /// <summary>
        /// 接收技能输入
        /// </summary>
        /// <returns></returns>
        public bool InputOk {
            get { return isInputOk; }
            set { isInputOk = false; }
        }
        
        /// <summary>
        /// 不接受输入
        /// </summary>
        /// <param CharacterName="time"></param>
        public void IgnoreInput(float time)
        {
            this.InputOk = false;
            MainLoop.Instance.ExecuteLater(_IgnoreInput, time);
        }


        #region 玩家专有属性

        #region 依赖资源的属性

        /// <summary>
        /// 玩家所剩余额
        /// </summary>
        public int Money
        {
            get { return playerInfo.money; }
            set { playerInfo.money = value; }
        }

        /// <summary>
        /// 玩家被击退速度
        /// </summary>
        public Vector2 HitBackSpeed
        {
            get { return playerInfo.hitBackSpeed; }
            set { playerInfo.hitBackSpeed = value; }
        }

        /// <summary>
        /// 最大药引上限
        /// </summary>
        public int Maxdrag
        {
            get { return playerInfo.Maxdrag; }
            set { playerInfo.Maxdrag = value; }
        }


        /// <summary>
        /// 技能位移
        /// </summary>
        public float AirXMove
        {
            get { return playerInfo.airXMove; }
            set { playerInfo.airXMove = value; }
        }

        /// <summary>
        /// 可承载最大灵器数量
        /// </summary>
        public int MaxSpiritNum
        {
            get { return playerInfo.MaxSpiritNum; }
            set { playerInfo.MaxSpiritNum = value; }
        }        

        #endregion
        
        #region 本地属性

        internal PlayerInfo playerInfo => characterInfoInfo as PlayerInfo;
        
        /// <summary>
        /// 当前药引
        /// </summary>
        public int Drag
        {
            get { return drag; }
            set { drag = value; }
        }        

        /// <summary>
        /// 玩家身上物品列表
        /// </summary>
        public List<ItemData> ItemDatas => playerInfo.itemList;

        /// <summary>
        /// 等级管理者
        /// </summary>
        public readonly RankMgr rankMgr = RankMgr.Instance;        
        

        #endregion

        #endregion

        #region 灵器

        public void AddSpirit(string spiritName)
        {
            var spiritDic = (characterInfoInfo as PlayerInfo).spiritDic;
            if (spiritDic.Count >= this.MaxSpiritNum)
            {
                Debug.Log("无法承载更多灵器");
                return;
            }

            var spirit = AbstractSpiritItem.GetInstance(spiritName);
            spiritDic.Add(spiritName, spirit);
            spirit.OnEnable();
        }

        public void RemoveSpirit(string spiritName)
        {
            var spiritDic = (characterInfoInfo as PlayerInfo).spiritDic;
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
            // if (Convert.ToSingle(this.Exp) / Convert.ToSingle(this.MaxExp) >= 1 / 3.0f && !isSuper)
            {
                //进入强化状态
                //可能要UI，动画，特效，移动，各个领域配合
                Debug.Log("Super");
                CEventCenter.BroadMessage(Message.M_InitSuper);
                MainLoop.Instance.ExecuteLater(() => { CEventCenter.BroadMessage(Message.M_ExitSuper); }, superTime);
            }
        }

        #endregion
        
        #region Private_Fields
        
        private bool isInputOk = true;
        private int drag = 0;
        private MainCharacter mainc;
        private CharacterController2D cc;
        
        #region 连击

        private int comboNum = 0;
        private float timer = 0;

        #endregion
        #endregion

        #region Override_Functions
        
        #region BattleEffect

        public override void TakeBattleEffect(List<IBattleEffect> beList)
        {
            base.TakeBattleEffect(beList);
            new AudioEffect("Fuck").Execute(this);
            new ShineEffect(this.DefaultConstTime, 0.1f, Color.blue).Execute(this);
            new ShakeScreenEffect(0.05f, Time.deltaTime).Execute(this);
        }


        protected override void AddBaseAttackEffects(AbstractPerson self)
        {
            var player = self as Player;
            Assert.IsTrue(player != null);
            player.attackEffects.Add(new InstantDamageEffect(
                GameMath.Damage((int) characterInfoInfo.baseAttack, characterInfoInfo.attack_scaler,
                    characterInfoInfo.attack_adder), player.Dir));
            player.attackEffects.Add(new HitbackEffect(new Vector2(player.Dir * Mathf.Abs(HitBackSpeed.x),
                HitBackSpeed.y)));
        }

        #endregion

        #region 战斗
        
        public override void OnCauseDamage(int damage)
        {
            base.OnCauseDamage(damage);
            Debug.Log("damage");
        }
        public override void DestoryThis()
        {
            base.DestoryThis();
            Debug.Log("主角死了");
        }

        #endregion        

        #region 更新和事件

        protected override void Update()
        {
            foreach (var VARIABLE in playerInfo.commonSkillInfos)
            {
                if (Input.GetKeyDown(VARIABLE.key))
                    VARIABLE.skillAsset.RunSkill(this);
            }

            //Z强化
            if (Input.GetKeyDown(playerInfo.superKey))
            {
                TrySuper();
            }

            //背包
            if (Input.GetKeyDown(playerInfo.bagKey))
            {
                PanelAssetMgr.PushPanel(playerInfo.packagePanelAsset);
            }

            if (Input.GetKeyUp(playerInfo.bagKey))
            {
                PanelAssetMgr.PopPanel();
            }

            #region 三连击

            ComboSkillInfo lastSkill = null;
            var index = this.comboNum - 1;

            if (this.comboNum > 0)
            {
                lastSkill = playerInfo.comboSkillInfos[this.comboNum - 1];

                Assert.IsTrue(this.comboNum > 0 && this.comboNum < 4);

                if (this.timer - lastSkill.StartTime > lastSkill.canMoveTime)
                {
//                    Debug.Log("过了移动锁定时间，恢复人物控制");
                    mainc._inControl = true;
                }

                if (this.timer - lastSkill.StartTime >
                    lastSkill.LastTime * lastSkill.beginComboTest + lastSkill.faultToleranceTime)
                {
                    this.comboNum = 0;
                    MainLoop.RemoveUpdateFunc(_DeUpdate);
                    //恢复人物控制
//                    Debug.Log("连击中断，恢复人物控制");
                    mainc._inControl = true;
                }
                else
                {
                }
            }

            if (Input.GetKeyDown(playerInfo.comboKey))
            {
//                Debug.Log("按键");
                if (!cc.isGrounded)
                {
                    mainc._playerVelocityY = 0;
                    this.TakeBattleEffect(new HitbackEffect(new Vector2(this.Dir * Mathf.Abs(this.AirXMove), 0)));
                }

//                Debug.Log("按下攻击键，无论如何，锁定人物");
                mainc._inControl = false;


                index++;
                if (index >= playerInfo.comboSkillInfos.Count)
                    return;


                ComboSkillInfo thisSkill = playerInfo.comboSkillInfos[index];


                if (this.comboNum == 0) //初次攻击
                {
                    this.timer = 0;
                    //开始增加时间
                    //                    Debug.Log("初次攻击");
                    MainLoop.AddUpdateFunc(_DeUpdate);
//                    Debug.Log("执行了技能" + index + " " + thisSkill.SkillName);
                    thisSkill.RunSkill(this, this.playerInfo.comboSkillInfos[0].ignoreInput, this.timer);

                    this.comboNum++;
                }

                else if (this.timer - lastSkill.StartTime <
                         lastSkill.LastTime * lastSkill.beginComboTest + lastSkill.faultToleranceTime)
                {
                    if (this.timer - lastSkill.StartTime > lastSkill.LastTime * lastSkill.beginComboTest)
                    {
                        if (this.comboNum < this.playerInfo.comboSkillInfos.Count)
                        {
//                             Debug.Log("执行了技能" + index + " " + thisSkill.SkillName);
                            thisSkill.RunSkill(this, lastSkill.ignoreInput, this.timer);
                            this.comboNum++;
                        }

//                         else
//                         {
//                             Debug.Log($"连击数大于攻击次数：this.comboNum:{comboNum}, 技能数量：{this.playerInfo.comboSkillInfos.Count}");
//                         }                       
                    }

//                    else
//                        Debug.Log($"还没有开始连击检测！ 距离上次攻击间隔：{this.timer-lastSkill.StartTime}, lastSkill.StartTime: {lastSkill.StartTime}，开始连击检测的间隔：{playerInfo.comboSkillInfos[this.comboNum-1].beginComboTest*lastSkill.LastTime}");
                }

//                else
//                {
//                    var skill = this.playerInfo.comboSkillInfos[comboNum - 1];
//                    Debug.Log($"技能开始时间：{skill.StartTime}, 技能持续时间：{skill.LastTime} 连击中止时间："+(skill.faultToleranceTime+skill.LastTime*skill.beginComboTest));
//                    Debug.Log($"已经超过连击检测容错时间： 上次释放技能到现在间隔：{this.timer-lastSkill.StartTime}, 容错时间：{ lastSkill.LastTime * this.playerInfo.comboSkillInfos[this.comboNum - 1].beginComboTest + this.playerInfo.comboSkillInfos[this.comboNum - 1].faultToleranceTime}");
//                }
            }

            #endregion
        }
        
        /// 这里只处理数值变化
        public override void OnAddListener()
        {
            base.OnAddListener();
            CEventCenter.AddListener(Message.M_InitSuper, InitSuper);
            CEventCenter.AddListener(Message.M_ExitSuper, ExitSuper);
            CEventCenter.AddListener<int>(Message.M_OnTryBut, OnTryBuy);
            CEventCenter.AddListener<int, int>(Message.M_AddItem, AddItem);
            CEventCenter.AddListener<int, int>(Message.M_RemoveItem, RemoveItem);
        }

        public override void OnRemoveListener()
        {
            base.OnRemoveListener();
            CEventCenter.RemoveListener(Message.M_InitSuper, InitSuper);
            CEventCenter.RemoveListener(Message.M_ExitSuper, ExitSuper);
            CEventCenter.RemoveListener<int>(Message.M_OnTryBut, OnTryBuy);
            CEventCenter.RemoveListener<int, int>(Message.M_AddItem, AddItem);
            CEventCenter.RemoveListener<int, int>(Message.M_RemoveItem, RemoveItem);
        }
        
        #endregion
        
        #endregion

        #region 构造和初始化        

        public Player()
        {
        }

        public Player(BaseCharacterInfo characterInfo,Vector3 pos, Transform parent = null) : base(characterInfo,pos, parent)
        {
            GlobalVar.G_Player = this;

            mainc = this.obj.GetComponent<MainCharacter>();
            Assert.IsTrue(mainc != null);
            cc = this.obj.GetComponent<CharacterController2D>();
            Assert.IsTrue(cc != null);

            this.BaseAttack = 20;
            this.MaxHp = 100;
        }

        #endregion
        
        #region Message_Handles
        private void OnTryBuy(int id)
        {
            var item = ItemInfoAsset.Instance.GetItem(id);
            if (item == null)
                throw new Exception("物品ID异常：id:" + id);

            Assert.IsTrue(item.Type == ItemType.Commercial);

            if (this.Money - item.Price < 0)
            {
                Debug.LogWarning("你只有: " + this.Money);
                return;
            }

            this.Money -= item.Price;
//            Debug.Log("当前金钱："+this.money);
            CEventCenter.BroadMessage(Message.M_MoneyChange, -item.Price);
            CEventCenter.BroadMessage(Message.M_AddItem, item.ID, 1);
        }

        void InitSuper()
        {
            isSuper = true;
        }

        void ExitSuper()
        {
            isSuper = false;
        }
        

        #endregion

        #region 内部调用

        private void _DeUpdate()
        {
            this.timer += Time.deltaTime;
        }
        private void _IgnoreInput()
        {
            this.InputOk = true;
        }
        #endregion

//        #region IXmlSerilize
//
//        public XmlSchema GetSchema()
//        {
//            return null;
//        }
//
//        public void ReadXml(XmlReader reader)
//        {
//            var floatSer = new XmlSerializer(typeof(float));
//            var intSer = new XmlSerializer(typeof(int));
//            var strSer = new XmlSerializer(typeof(string));
//            var vector3Ser = new XmlSerializer(typeof(Vector3));
//            var strListSer = new XmlSerializer(typeof(List<string>));
//
//            reader.Read();
//            reader.ReadStartElement("CharacterName");
//            this.CharacterName = (string)strSer.Deserialize(reader);
//            reader.ReadEndElement();
//
//            reader.ReadStartElement("Pos");
//            var pos = (Vector3)vector3Ser.Deserialize(reader);
//            this.obj = Resources.Load<GameObject>(DefaultData.PlayerPath);
//            this.obj = UnityEngine.Object.Instantiate(this.obj, pos, Quaternion.identity);
//            reader.ReadEndElement();
//
//            if (this.obj == null)
//                Debug.LogError("主角为空了，干");
//
//            reader.ReadStartElement("BaseAttack");
//            this.BaseAttack = (int)intSer.Deserialize(reader);
//            reader.ReadEndElement();
//
//            reader.ReadStartElement("Skills");
//            this.allSkillNames = (List<string>)strListSer.Deserialize(reader);
//            foreach (var skill in allSkillNames)
//                this.skills.Add(SkillTriggerMgr.skillInstanceDic[skill]);
//            reader.ReadEndElement();
//
//            attackEffects = new List<IBattleEffect>();
//            this.InputOk = true;
//            this.IsConst = false;
//            this.IgnoreHitback = false;
//
//            this.comboNum = 0;
//            this.timer = 0;
//
//            this.beginComboTest = new List<float>();//0~1之间
//            this.durTimes = new List<float>();
//            this.canMoveTime = new List<float>();
//            this.skillNames = new List<string>();
//            this.ignoreInput = new List<bool>();
//
//            GlobalVar.G_Player = this;
//            mainc = this.obj.GetComponent<MainCharacter>();
//            Assert.IsTrue(mainc != null);
//            cc = this.obj.GetComponent<CharacterController2D>();
//            Assert.IsTrue(cc != null);
//            this.DefaultConstTime = 0.7f;
//            this.attackMultipulier = 1;  //攻击力翻倍
//            this.attackAdder = 0;        //攻击力叠加
//
//            reader.ReadStartElement("HitbackSpeed");
//            this.HitBackSpeed = (float)floatSer.Deserialize(reader);
//            reader.ReadEndElement();
//
//
//            //添加基本攻击效果
//            this.OnAttackListRefresh += AddBaseAttackEffects;
//
//            //添加事件监听
//            OnAddListener();
//
//            //开始每帧Update
//            MainLoop.AddUpdateFunc(Update);
//
//            //记录每个实例
//            instanceList.Add(this);
//            reader.ReadEndElement();
//        }
//
//        public void WriteXml(XmlWriter writer)
//        {
//            //            Debug.Log("这真的调用了么");
//            var floatSer = new XmlSerializer(typeof(float));
//            var intSer = new XmlSerializer(typeof(int));
//            var strSer = new XmlSerializer(typeof(string));
//            var vector3Ser = new XmlSerializer(typeof(Vector3));
//            var strListSer = new XmlSerializer(typeof(List<string>));
//            //            var boolSer = new XmlSerializer(typeof(bool));
//            //            var boolListSer=new XmlSerializer(typeof(List<bool>));
//            //            var floatListSer=new XmlSerializer(typeof(List<float>));
//            //            var skillDicSer=new XmlSerializer(typeof(SerializableDictionary<string,SkillInstance>));
//
//            writer.WriteStartElement("CharacterName");
//            strSer.Serialize(writer, this.CharacterName);
//            writer.WriteEndElement();
//
//            writer.WriteStartElement("Pos");
//            vector3Ser.Serialize(writer, this.Pos);
//            writer.WriteEndElement();
//
//
//            writer.WriteStartElement("BaseAttack");
//            intSer.Serialize(writer, this.BaseAttack);
//            writer.WriteEndElement();
//
//            writer.WriteStartElement("Skills");
//            strListSer.Serialize(writer, this.allSkillNames);
//            writer.WriteEndElement();
//
//            writer.WriteStartElement("HitbackSpeed");
//            floatSer.Serialize(writer, this.HitBackSpeed);
//            writer.WriteEndElement();
//
//
//            //            writer.WriteStartElement("Skills");
//            //            skillDicSer.Serialize(writer,this.skillDic);
//
//
//        }
//
//        XmlSchema IXmlSerializable.GetSchema()
//        {
//            throw new NotImplementedException();
//        }
//
//        #endregion
    }
}