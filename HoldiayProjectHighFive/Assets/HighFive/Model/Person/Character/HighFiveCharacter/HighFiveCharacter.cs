using System;
using System.Collections.Generic;
using HighFive.Const;
using HighFive.Data;
using HighFive.Global;
using HighFive.Model.ItemSystem;
using HighFive.Model.RankSystem;
using HighFive.Script;
using ReadyGamerOne.Common;
using ReadyGamerOne.Data;
using ReadyGamerOne.Rougelike.Item;
using ReadyGamerOne.Rougelike.Person;
using ReadyGamerOne.Script;
using ReadyGamerOne.Utility;
using ReadyGamerOne.View;
using UnityEngine;
using UnityEngine.Assertions;
using Random = UnityEngine.Random;

namespace HighFive.Model.Person
{       
	public interface IHighFiveCharacter : 
		IHighFivePerson,
		IRankablePerson,
		IItemablePerson<HighFiveItem>
	{
		#region 金钱

		int Money { get;}
		int ChangeMoney(int change);		

		#endregion

		#region 药引

		int Drag { get; }
		int ChangeExp(int change);
		int MaxDrag { get; }		

		#endregion

		int ChangeDrag(int change);
		

		int BaseAttack { get; }
		
		float AirXMove { get; set; }
		int MaxSpiritNum { get; set; }
		
		bool InputOk { get; }
		void IgnoreInput(float time);
	}

	public abstract class HighFiveCharacter<T>:
		HighFivePerson<T>,
		IHighFiveCharacter
		where T : HighFiveCharacter<T>,new()
	{
		#region GUI

		public GUIStyle style;
		
		#endregion
		
		#region Fields

		protected RankMgr _rankMgr = RankMgr.Instance;
		private bool isInputOk = true;
		private int comboNum = 0;
		private float timer = 0;
		
		#endregion

		protected HighFiveCharacterController CharacterController => Controller as HighFiveCharacterController;
		
		#region 背包
		
		protected Dictionary<string,HighFiveItem> itemDic = new Dictionary<string, HighFiveItem>();

		#region IItemablePerson<HighFiveItem>

		public HighFiveItem AddItem(string itemId, int count = 1)
		{
			if (count < 1)
			{
				Debug.LogWarning("添加了 "+count+" 物品");
				return null;
			}

			if (GetItemCount(itemId) == 0)
			{
				//这里直接使用ItemInfo是因为整个项目ItemInfo时所有物品信息基类
				var itemData = CsvMgr.GetData<ItemData>(itemId);
				if (null == itemData)
				{
					throw new Exception("异常ID: " + itemId);
				}

				var itemInfo =itemData.CreateItem<HighFiveItem>(count);
//				info.RefreshCharacterProperties();
				if(null==itemInfo)
					throw new Exception("iteminfo is not ItemData");
//				itemInfo.OnEnable(this);
				itemDic.Add(itemId,itemInfo);
                
			}
			else
				itemDic[itemId].Count += count;
            

			return itemDic[itemId];
		}

		public HighFiveItem RemoveItem(string itemId, int count = 1)
		{
			var curcount = GetItemCount(itemId);

			if (curcount == 0)
				return null;
            
			if (curcount <= count)
			{
				var info = itemDic[itemId];
				info.Count = 0;
//				info.RefreshCharacterProperties();
//				info.OnDisable();
				itemDic.Remove(itemId);
				return itemDic[itemId];
			}

			itemDic[itemId].Count -= count;
//			itemDic[itemId].RefreshCharacterProperties();
			return itemDic[itemId];
		}

		public Dictionary<string,HighFiveItem> GetItems()
		{
			return itemDic;
		}

		public int GetItemCount(string itemId)
		{
			if (!itemDic.ContainsKey(itemId))
				return 0;
			return itemDic[itemId].Count;
		}      		

		#endregion
		
		#endregion

		#region IHighFiveCharacter
		public int ChangeExp(int change)
		{
			var levelUp = false;
			var extraExp = Drag + change - this.MaxDrag;


			if(extraExp >= 0)
			{
				levelUp =_rankMgr.TryLevelUp(extraExp,
					rankData =>
					{
						
					});
			}


			if (!levelUp)
			{// 升级失败
				Drag = Mathf.Clamp(Drag + change, 0, this.MaxDrag);
//				Debug.Log($"升级失败：【{Drag}】");
			}
			else
			{
				CEventCenter.BroadMessage(Message.M_LevelUp);
//				Debug.Log($"升级成功：【{Drag}】");
			}
			
			
			return Drag;
		}
		
		public int BaseAttack => _rankMgr.BaseAttack;
		
		/// <summary>
		/// 可承载最大灵器数量
		/// </summary>
		public int MaxSpiritNum
		{
			get { return CharacterController.MaxSpiritNum; }
			set { CharacterController.MaxSpiritNum = value; }
		}

		#region IUseCsvData

		public override Type DataType => typeof(CharacterData);


		public override void LoadData(CsvMgr data)
		{
			base.LoadData(data);
			var characterData = data as CharacterData;
			Assert.IsNotNull(characterData);
			DefaultConstTime = characterData.defaultConstTime;
		}

		#endregion

		#region ITakeDamageablePerson<AbstractPerson>
		
		

		public override float OnTakeDamage(AbstractPerson takeDamageFrom, BasicDamage damage)
        {
	        var realDamage = base.OnTakeDamage(takeDamageFrom, damage);
	        if (realDamage > 0)
	        {
		        CEventCenter.BroadMessage(Message.M_PlayerBloodChange,-Mathf.RoundToInt(realDamage));
		        this.IsInvincible = true;
		        MainLoop.Instance.ExecuteLater(() => IsInvincible = false, this.DefaultConstTime);
	        }
            return realDamage;
        }

		public override float OnCauseDamage(AbstractPerson causeDamageTo, BasicDamage damage)
		{
			var realDamage= base.OnCauseDamage(causeDamageTo, damage);
			if (realDamage > 0 && System.Math.Abs(realDamage) > 0.01f)
			{
				var change = Mathf.RoundToInt(realDamage * GameSettings.Instance.damageToDragScale);
				CEventCenter.BroadMessage(Message.M_PlayerDragChange,change);
			}

//			Debug.Log($"FinalDamage:[{realDamage}]");
			return realDamage;
		}

		#endregion
		
		#region Hp_Attack_使用rankMgr

		public override int Hp
		{
			get { return _rankMgr.Hp; }
			protected set { _rankMgr.Hp = value; }
		}

		public override int MaxHp
		{
			get
			{
				return _rankMgr.MaxHp;	
			}
			protected set { throw new Exception("HighFiveCharacter不能手动设置MaxHp"); }
		}

		public override int Attack
		{
			get
			{
				var normalDamage = (_rankMgr.BaseAttack + AttackAdder) * AttackScale;
				return  Mathf.RoundToInt(normalDamage);
			}
		}
		

		#endregion
		
		#region Money

		/// <summary>
		/// 玩家所剩余额
		/// </summary>
		public int Money
		{
			get { return CharacterController.money; }
			set { CharacterController.money = value; }
		}

		public int ChangeMoney(int change)
		{
			this.Money = Mathf.Clamp(this.Money + change, 0, int.MaxValue);
			return this.Money;
		}		

		#endregion		
		
		#region Drag

		private int _drag;
		
		/// <summary>
		/// 当前药引
		/// </summary>
		public int Drag
		{
			get { return _drag; }
			private set { _drag = value; }
		}
		
		/// <summary>
		/// 最大药引上限
		/// </summary>
		public int MaxDrag => 100;

		/// <summary>
		/// 改变药引
		/// </summary>
		/// <param name="change"></param>
		/// <returns>返回-1表示无法使用</returns>
		public int ChangeDrag(int change)
		{
			if (change < 0 && Drag < -change)
			{
				//表示无法使用
				return -1;
			}

			Drag = Mathf.Clamp(Drag + change, 0, MaxDrag);
			return Drag;
		}


		#endregion
		
		#region IRankablePerson

		public int Exp
		{
			get
			{
				return _rankMgr.Exp;
			}
			private set { _rankMgr.Exp = value; }
		} 
		public int MaxExp => _rankMgr.MaxExp;
		public string Rank => _rankMgr.Rank;
		public bool TryLevelUp(int extraExp)
		{
			return _rankMgr.TryLevelUp(extraExp,
				rankInfo =>
				{
					//玩家增益
				});
		}		

		#endregion


		public override void OnInstanciateObject()
		{
			base.OnInstanciateObject();           
			style = new GUIStyle
			{
				fontSize = 50
			};
			style.normal.textColor = Color.cyan;
			style.alignment = TextAnchor.MiddleCenter;
		}

		#region IPoolable<PoolPerson<T>>

		public override void OnGetFromPool()
		{
			base.OnGetFromPool();
			Debug.Log($"{CharacterName} GetFromPool");

			// 设置全局变量
			if(GlobalVar.G_Player==null)
				GlobalVar.SetPlayer(this);
			
			//根据缓存回复位置
			position = DefaultData.PlayerPos;
			
			CEventCenter.AddListener(Message.M_ExitSuper, ExitSuper);
            CEventCenter.AddListener<string>(Message.M_OnTryBut, OnTryBuy);
            CEventCenter.AddListener<string, int>(Message.M_AddItem, AddItemFromMessage);
            CEventCenter.AddListener<string, int>(Message.M_RemoveItem, RemoveItemFromMessage);
            
            // OnGUI
            MainLoop.Instance.AddGUIFunc(OnGUI);
		}

		public override void OnRecycleToPool()
		{
			base.OnRecycleToPool();
			Debug.Log($"{CharacterName} RecycleToPool");
			
			CEventCenter.RemoveListener(Message.M_ExitSuper, ExitSuper);
			CEventCenter.RemoveListener<string>(Message.M_OnTryBut, OnTryBuy);
			CEventCenter.RemoveListener<string, int>(Message.M_AddItem, AddItemFromMessage);
			CEventCenter.RemoveListener<string, int>(Message.M_RemoveItem, RemoveItemFromMessage);
			
			// OnGUI
			MainLoop.Instance.RemoveGUIFunc(OnGUI);
		}

		#endregion
		
		#region 技能与控制

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
		/// <param CharacterName="inTime"></param>
		public void IgnoreInput(float time)
		{
			this.InputOk = false;
			MainLoop.Instance.ExecuteLater(_IgnoreInput, time);
		}		
		

		/// <summary>
		/// 技能位移
		/// </summary>
		public float AirXMove
		{
			get { return CharacterController.airXMove; }
			set { CharacterController.airXMove = value; }
		}		

		#endregion
		
		
		#endregion

		#region 消耗灵力的强化系统

		private bool TrySuper()
		{
			if (!InitSuper()) 
				return false; 
			
			// 进入强化状态
				
			//通知UI，动画，特效，移动，各个领域配合
			CEventCenter.BroadMessage(Message.M_InitSuper);
			MainLoop.Instance.ExecuteLater(() =>
			{
				if(GlobalVar.isSuper)
					CEventCenter.BroadMessage(Message.M_ExitSuper);
			}, GameSettings.Instance.superTime);
			return true;

		}
		#endregion
		
		#region 更新与事件

		protected override void Update()
        {
            foreach (var VARIABLE in CharacterController.commonSkillInfos)
            {
                if (Input.GetKeyDown(VARIABLE.key))
                    VARIABLE.skillAsset.RunSkill(this);
            }

            //Z强化
            if (Input.GetKeyDown(CharacterController.superKey))
            {
                TrySuper();
            }

            //背包
            if (Input.GetKeyDown(CharacterController.bagKey))
            {
                PanelMgr.PushPanel(PanelName.PackagePanel);
            }

            if (Input.GetKeyUp(CharacterController.bagKey))
            {
                PanelMgr.PopPanel();
            }

            #region 三连击

            ComboSkillInfo lastSkill = null;
            var index = this.comboNum - 1;

            if (this.comboNum > 0)
            {
                lastSkill = CharacterController.comboSkillInfos[this.comboNum - 1];

                Assert.IsTrue(this.comboNum > 0 && this.comboNum < 4);

                if (this.timer - lastSkill.StartTime > lastSkill.CanMoveTime)
                {
//                    Debug.Log("过了移动锁定时间，恢复人物控制");
                    Controller.SetMoveable(true);
                }

                if (this.timer - lastSkill.StartTime >
                    lastSkill.LastTime * lastSkill.BeginComboTest + lastSkill.FaultToleranceTime)
                {
                    this.comboNum = 0;
                    MainLoop.Instance.RemoveUpdateFunc(_DeUpdate);
                    //恢复人物控制
//                    Debug.Log("连击中断，恢复人物控制");
                    Controller.SetMoveable(true);
                }
            }

            if (Input.GetKeyDown(CharacterController.comboKey))
            {
//                Debug.Log("按键");
                if (!ActorMover.IsGrounded)
                {
	                ActorMover.VelocityY = 0;
//                    this.TakeBattleEffect(new HitbackEffect(new Vector2(this.FaceDir * Mathf.Abs(this.AirXMove), 0)));
                }

//                Debug.Log("按下攻击键，无论如何，锁定人物");
                Controller.SetMoveable(false);


                index++;
                if (index >= CharacterController.comboSkillInfos.Count)
                {
//	                Debug.Log($"Index : {index} Count: {CharacterController.comboSkillInfos.Count}");
	                return;
                }


                var thisSkill = CharacterController.comboSkillInfos[index];


                if (this.comboNum == 0) //初次攻击
                {
                    this.timer = 0;
                    //开始增加时间
                    //                    Debug.Log("初次攻击");
                    MainLoop.Instance.AddUpdateFunc(_DeUpdate);
//                    Debug.Log("执行了技能" + index + " " + thisSkill.SkillName);
                    thisSkill.RunSkill(this, CharacterController.comboSkillInfos[0].IgnoreInput, this.timer);

                    this.comboNum++;
                }

                else if (this.timer - lastSkill.StartTime <
                         lastSkill.LastTime * lastSkill.BeginComboTest + lastSkill.FaultToleranceTime)
                {
                    if (this.timer - lastSkill.StartTime > lastSkill.LastTime * lastSkill.BeginComboTest)
                    {
                        if (this.comboNum < CharacterController.comboSkillInfos.Count)
                        {
//                             Debug.Log("执行了技能" + index + " " + thisSkill.SkillName);
                            thisSkill.RunSkill(this, lastSkill.IgnoreInput, this.timer);
                            this.comboNum++;
                        }
//                        else
//                        {
//                             Debug.Log($"连击数大于攻击次数：this.comboNum:{comboNum}, 技能数量：{CharacterController.comboSkillInfos.Count}");
//                        }                       
                    }
//
//                    else
//                    {
//	                    Debug.Log("lastSkill.Name: "+lastSkill.SkillName+"  lastSkill.lastTime: "+lastSkill.LastTime);
//                        Debug.Log($"还没有开始连击检测！ 距离上次攻击间隔：{this.timer-lastSkill.StartTime}, lastSkill.StartTime: {lastSkill.StartTime}，开始连击检测的间隔：{CharacterController.comboSkillInfos[this.comboNum-1].beginComboTest*lastSkill.LastTime}");
//                    }
                }
//                else
//                {
//                    var skill = CharacterController.comboSkillInfos[comboNum - 1];
//                    Debug.Log($"技能开始时间：{skill.StartTime}, 技能持续时间：{skill.LastTime} 连击中止时间："+(skill.faultToleranceTime+skill.LastTime*skill.beginComboTest));
//                    Debug.Log($"已经超过连击检测容错时间： 上次释放技能到现在间隔：{this.timer-lastSkill.StartTime}, 容错时间：{ lastSkill.LastTime * CharacterController.comboSkillInfos[this.comboNum - 1].beginComboTest + CharacterController.comboSkillInfos[this.comboNum - 1].faultToleranceTime}");
//                }
            }

            #endregion
        }
		

		#region Message_Handles

		private void AddItemFromMessage(string id, int count)
		{
			Debug.Log($"GetItem{id}:[{count}]");
			AddItem(id, count);
		}

		private void RemoveItemFromMessage(string id, int count) => RemoveItem(id, count);
		
		
		private void OnTryBuy(string id)
		{
			var itemData = CsvMgr.GetData<ItemData>(id);
			if (itemData == null)
				throw new Exception("物品ID异常：id:" + id);

//			Assert.IsTrue(ItemData.Type == ItemType.Commercial);

			if (this.Money - itemData.price < 0)
			{
				Debug.LogWarning("你只有: " + this.Money);
				return;
			}

			this.Money -= itemData.price;
//            Debug.Log("当前金钱："+this.money);
			CEventCenter.BroadMessage(Message.M_MoneyChange, -itemData.price);
			CEventCenter.BroadMessage(Message.M_AddItem, itemData.ID, 1);
		}

		/// <summary>
		/// 如果可以强化，就进入强化状态
		/// </summary>
		/// <returns></returns>
		private bool InitSuper()
		{
			var dragConsume = Mathf.RoundToInt(GameSettings.Instance.superDragConsumeRate * this.MaxDrag);
			if (Drag > dragConsume && !GlobalVar.isSuper)
			{
				GlobalVar.isSuper = true;
				return true;
			}

			return false;
		}

		/// <summary>
		/// 退出强化状态
		/// </summary>
		void ExitSuper()
		{
			if (GlobalVar.isSuper)
			{
				GlobalVar.isSuper = false;
				CEventCenter.BroadMessage(Message.M_PlayerDragChange,
					-GameSettings.Instance.superDragConsumeRate*MaxDrag);
			}
		}
        

		#endregion
		#endregion
		
		#region 内部调用

		private void _DeUpdate()
		{
//			Debug.Log("DeUpdate");
			this.timer += Time.deltaTime;
		}
		private void _IgnoreInput()
		{
			this.InputOk = true;
		}

		private void OnGUI()
		{
			if (!IsAlive)
				return;
			GUILayout.Space(40);
			style.fontSize = (int)GUILayoutUtil.Slider("字体大小", style.fontSize, 0, 100, style);
			GUILayout.Label($"基础攻击\t【{this.BaseAttack}】",style);
			this.AttackAdder = GUILayoutUtil.Slider("攻击加成", this.AttackAdder, 0, 100, style);
			this.AttackScale = GUILayoutUtil.Slider("攻击倍率", this.AttackScale, 0, 5, style);
			this.AttackSpeed = GUILayoutUtil.Slider("攻速", this.AttackSpeed, 0, 5, style);
			this.CritRate = GUILayoutUtil.Slider("暴击率", this.CritRate, 0, 1, style);
			this.CritScale = GUILayoutUtil.Slider("暴击倍率", this.CritScale, 0, 5, style);
			this.TakeDamageScale = GUILayoutUtil.Slider("承伤倍率", this.TakeDamageScale, 0, 2, style);
			this.TakeDamageBlock = GUILayoutUtil.Slider("固定格挡", this.TakeDamageBlock, 0, 50, style);
			this.DodgeRate = GUILayoutUtil.Slider("闪避", this.DodgeRate, 0, 1, style);
			this.RepulseScale = GUILayoutUtil.Slider("击退强度", this.RepulseScale, 0, 5, style);
			this.DefaultConstTime = GUILayoutUtil.Slider("无敌时间", this.DefaultConstTime, 0, 5, style);
			GUILayout.Label($"无敌\t【{this.IsInvincible}】",style);
			GUILayout.Label($"Z强化\t【{GlobalVar.isSuper}】",style);
		}
		
		#endregion
	}
}
