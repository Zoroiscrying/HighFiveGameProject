using System;
using System.Collections.Generic;
using HighFive.Const;
using HighFive.Data;
using HighFive.Global;
using HighFive.Model.ItemSystem;
using HighFive.Model.RankSystem;
using ReadyGamerOne.Common;
using ReadyGamerOne.Data;
using ReadyGamerOne.Rougelike.Item;
using ReadyGamerOne.Rougelike.Person;
using ReadyGamerOne.Script;
using ReadyGamerOne.View;
using UnityEngine;
using UnityEngine.Assertions;

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
		int ChangeDrag(int change);
		int MaxDrag { get; }		

		#endregion

		float AirXMove { get; set; }
		int MaxSpiritNum { get; set; }
				
		int AttackAdder { get; set; }
		float AttackScaler { get; set; }
		
		bool InputOk { get; }
		void IgnoreInput(float time);
	}

	public abstract class HighFiveCharacter<T>:
		HighFivePerson<T>,
		IHighFiveCharacter
		where T : HighFiveCharacter<T>,new()
	{
		#region IUseCsvData

		public override Type DataType => typeof(CharacterData);

		#endregion
		
		#region Fields

		protected RankMgr _rankMgr = RankMgr.Instance;

		#endregion

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


		#region ITakeDamageablePerson<AbstractPerson>

		public override void OnTakeDamage(AbstractPerson takeDamageFrom, int damage)
        {
        	if (IsConst)
        		return;
        	base.OnTakeDamage(takeDamageFrom, damage);
        	CEventCenter.BroadMessage(Message.M_PlayerBloodChange,-Mathf.Abs(damage));
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
			get => Mathf.RoundToInt((_rankMgr.BaseAttack+AttackAdder)*AttackScaler);
			protected set => throw new Exception("HighFiveCharacter禁止使用Attack的Set方法");
		}
		
		public int AttackAdder { get; set; } = 0;

		public float AttackScaler { get; set; } = 1;

		#endregion
		
		#region Money

		/// <summary>
		/// 玩家所剩余额
		/// </summary>
		public int Money
		{
			get { return (Controller as HighFiveCharacterController).money; }
			set { (Controller as HighFiveCharacterController).money = value; }
		}

		public int ChangeMoney(int change)
		{
			this.Money = Mathf.Clamp(this.Money + change, 0, int.MaxValue);
			return this.Money;
		}		

		#endregion
		
		#region Drag

		public int Drag
		{
			get { return Exp; }
			private set { Exp = value; }
		}

		public int ChangeDrag(int change)
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

		/// <summary>
		/// 最大药引上限
		/// </summary>
		public int MaxDrag => MaxExp;		

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
		
		#region IPoolable<PoolPerson<T>>

		public override void OnGetFromPool()
		{
			base.OnGetFromPool();	
			GlobalVar.G_Player = this;
                                 			
            CEventCenter.AddListener(Message.M_InitSuper, InitSuper);
            CEventCenter.AddListener(Message.M_ExitSuper, ExitSuper);
            CEventCenter.AddListener<string>(Message.M_OnTryBut, OnTryBuy);
            CEventCenter.AddListener<string, int>(Message.M_AddItem, (id,count)=>AddItem(id,count));
            CEventCenter.AddListener<string, int>(Message.M_RemoveItem, (id,count)=>RemoveItem(id,count));
		}

		public override void OnRecycleToPool()
		{
			base.OnRecycleToPool();
			CEventCenter.RemoveListener(Message.M_InitSuper, InitSuper);
			CEventCenter.RemoveListener(Message.M_ExitSuper, ExitSuper);
			CEventCenter.RemoveListener<string>(Message.M_OnTryBut, OnTryBuy);

//			if(GlobalVar.G_Player==this)
//				GlobalVar.G_Player = null;			
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
			get { return (Controller as HighFiveCharacterController).airXMove; }
			set { (Controller as HighFiveCharacterController).airXMove = value; }
		}		

		#endregion
		

		/// <summary>
		/// 可承载最大灵器数量
		/// </summary>
		public int MaxSpiritNum
		{
			get { return (Controller as HighFiveCharacterController).MaxSpiritNum; }
			set { (Controller as HighFiveCharacterController).MaxSpiritNum = value; }
		} 		

		
		#endregion

		#region 消耗灵力的强化系统

		private void TrySuper()
		{
			// if (Convert.ToSingle(this.Exp) / Convert.ToSingle(this.MaxExp) >= 1 / 3.0f && !isSuper)
			{
				//进入强化状态
				//可能要UI，动画，特效，移动，各个领域配合
				Debug.Log("Super");
				CEventCenter.BroadMessage(Message.M_InitSuper);
				MainLoop.Instance.ExecuteLater(() => { CEventCenter.BroadMessage(Message.M_ExitSuper); }, GlobalVar.superTime);
			}
		}

		#endregion
		
		#region Private_Fields
        
		private bool isInputOk = true;

		#region 连击

		private int comboNum = 0;
		private float timer = 0;

		#endregion
        
		#endregion
		
		#region 更新与事件

		protected override void Update()
        {
            foreach (var VARIABLE in (Controller as HighFiveCharacterController).commonSkillInfos)
            {
                if (Input.GetKeyDown(VARIABLE.key))
                    VARIABLE.skillAsset.RunSkill(this);
            }

            //Z强化
            if (Input.GetKeyDown((Controller as HighFiveCharacterController).superKey))
            {
                TrySuper();
            }

            //背包
            if (Input.GetKeyDown((Controller as HighFiveCharacterController).bagKey))
            {
                PanelMgr.PushPanel(PanelName.PackagePanel);
            }

            if (Input.GetKeyUp((Controller as HighFiveCharacterController).bagKey))
            {
                PanelMgr.PopPanel();
            }

            #region 三连击

            ComboSkillInfo lastSkill = null;
            var index = this.comboNum - 1;

            if (this.comboNum > 0)
            {
                lastSkill = (Controller as HighFiveCharacterController).comboSkillInfos[this.comboNum - 1];

                Assert.IsTrue(this.comboNum > 0 && this.comboNum < 4);

                if (this.timer - lastSkill.StartTime > lastSkill.canMoveTime)
                {
//                    Debug.Log("过了移动锁定时间，恢复人物控制");
                    Controller.SetMoveable(true);
                }

                if (this.timer - lastSkill.StartTime >
                    lastSkill.LastTime * lastSkill.beginComboTest + lastSkill.faultToleranceTime)
                {
                    this.comboNum = 0;
                    MainLoop.Instance.RemoveUpdateFunc(_DeUpdate);
                    //恢复人物控制
//                    Debug.Log("连击中断，恢复人物控制");
                    Controller.SetMoveable(true);
                }
            }

            if (Input.GetKeyDown((Controller as HighFiveCharacterController).comboKey))
            {
//                Debug.Log("按键");
                if (!(Controller as HighFiveCharacterController).characterController.isGrounded)
                {
	                ((Controller as HighFiveCharacterController).actor as MainCharacter)._playerVelocityY = 0;
//                    this.TakeBattleEffect(new HitbackEffect(new Vector2(this.Dir * Mathf.Abs(this.AirXMove), 0)));
                }

//                Debug.Log("按下攻击键，无论如何，锁定人物");
                Controller.SetMoveable(false);


                index++;
                if (index >= (Controller as HighFiveCharacterController).comboSkillInfos.Count)
                {
//	                Debug.Log($"Index : {index} Count: {(Controller as HighFiveCharacterController).comboSkillInfos.Count}");
	                return;
                }


                var thisSkill = (Controller as HighFiveCharacterController).comboSkillInfos[index];


                if (this.comboNum == 0) //初次攻击
                {
                    this.timer = 0;
                    //开始增加时间
                    //                    Debug.Log("初次攻击");
                    MainLoop.Instance.AddUpdateFunc(_DeUpdate);
//                    Debug.Log("执行了技能" + index + " " + thisSkill.SkillName);
                    thisSkill.RunSkill(this, (Controller as HighFiveCharacterController).comboSkillInfos[0].ignoreInput, this.timer);

                    this.comboNum++;
                }

                else if (this.timer - lastSkill.StartTime <
                         lastSkill.LastTime * lastSkill.beginComboTest + lastSkill.faultToleranceTime)
                {
                    if (this.timer - lastSkill.StartTime > lastSkill.LastTime * lastSkill.beginComboTest)
                    {
                        if (this.comboNum < (Controller as HighFiveCharacterController).comboSkillInfos.Count)
                        {
//                             Debug.Log("执行了技能" + index + " " + thisSkill.SkillName);
                            thisSkill.RunSkill(this, lastSkill.ignoreInput, this.timer);
                            this.comboNum++;
                        }
//                        else
//                        {
//                             Debug.Log($"连击数大于攻击次数：this.comboNum:{comboNum}, 技能数量：{(Controller as HighFiveCharacterController).comboSkillInfos.Count}");
//                        }                       
                    }
//
//                    else
//                    {
//	                    Debug.Log("lastSkill.Name: "+lastSkill.SkillName+"  lastSkill.lastTime: "+lastSkill.LastTime);
//                        Debug.Log($"还没有开始连击检测！ 距离上次攻击间隔：{this.timer-lastSkill.StartTime}, lastSkill.StartTime: {lastSkill.StartTime}，开始连击检测的间隔：{(Controller as HighFiveCharacterController).comboSkillInfos[this.comboNum-1].beginComboTest*lastSkill.LastTime}");
//                    }
                }
//                else
//                {
//                    var skill = (Controller as HighFiveCharacterController).comboSkillInfos[comboNum - 1];
//                    Debug.Log($"技能开始时间：{skill.StartTime}, 技能持续时间：{skill.LastTime} 连击中止时间："+(skill.faultToleranceTime+skill.LastTime*skill.beginComboTest));
//                    Debug.Log($"已经超过连击检测容错时间： 上次释放技能到现在间隔：{this.timer-lastSkill.StartTime}, 容错时间：{ lastSkill.LastTime * (Controller as HighFiveCharacterController).comboSkillInfos[this.comboNum - 1].beginComboTest + (Controller as HighFiveCharacterController).comboSkillInfos[this.comboNum - 1].faultToleranceTime}");
//                }
            }

            #endregion
        }
		

		#endregion
		
		#region Message_Handles
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

		void InitSuper()
		{
			GlobalVar.isSuper = true;
		}

		void ExitSuper()
		{
			GlobalVar.isSuper = false;
		}
        

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
		#endregion
	}
}
