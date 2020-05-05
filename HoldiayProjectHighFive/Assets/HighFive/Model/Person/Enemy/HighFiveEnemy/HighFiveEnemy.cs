using ReadyGamerOne.Rougelike.Person;
using System;
using Game.Scripts;
using HighFive.Const;
using HighFive.Data;
using HighFive.Global;
using ReadyGamerOne.Common;
using ReadyGamerOne.Data;
using ReadyGamerOne.MemorySystem;
using ReadyGamerOne.Script;
using ReadyGamerOne.Scripts;
using UnityEngine;
using UnityEngine.Assertions;
using ReadyGamerOne.Utility;
using Random = UnityEngine.Random;

namespace HighFive.Model.Person
{
	public interface IHighFiveEnemy : 
		IHighFivePerson
	{
		HeadUiCanvas HeadUi { get; }
	}

	public abstract class HighFiveEnemy<T>:
		HighFivePerson<T>,
		IHighFiveEnemy
		where T : HighFiveEnemy<T>,new()
	{
		#region Fields

		
		/// <summary>
		/// 头顶UI
		/// </summary>
		protected SuperBloodBar headBloodBar;


		#endregion

		#region IUseCsvData

		public override Type DataType => typeof(EnemyData);

		public override void LoadData(CsvMgr data)
		{
			base.LoadData(data);
			var enemyData = CsvData as EnemyData;
			if (null == enemyData)
			{
				throw new Exception("获取敌方Data为空");
			}

			this._attack = enemyData.attack;
			this.MaxHp = enemyData.maxHp;
			this.Hp = this.MaxHp;
		}		

		#endregion

		
		private int _attack;
		
		public HeadUiCanvas HeadUi => (Controller as HighFiveEnemyController).HeadUi;
		
		public override void OnInstanciateObject()
		{
			base.OnInstanciateObject();
			Assert.IsTrue(HeadUi);
			headBloodBar = HeadUi.GetComponent<SuperBloodBar>("SuperBloodBar");
			Assert.IsTrue(headBloodBar);
		}
		
		public override void OnGetFromPool()
		{
			base.OnGetFromPool();
			HeadUi.GetComponent<Canvas>().enabled = true;
			headBloodBar.InitValue(MaxHp);
		}

		public override float OnTakeDamage(AbstractPerson takeDamageFrom, float damage)
		{
			var realDamage = base.OnTakeDamage(takeDamageFrom, damage);
			if(realDamage>0)
				headBloodBar.Value -= realDamage;
			return realDamage;
		}


		public override int Attack
		{
			get
			{
				var normalDamage =(_attack + AttackAdder) * AttackScale;
				if (Random.Range(0, 1f) < this.CritRate)
					normalDamage *= CritScale;
				return Mathf.RoundToInt(normalDamage);
			} 
		}

		public override void Kill()
		{
			DropItems(this.CharacterName);
			base.Kill();
			HeadUi.GetComponent<Canvas>().enabled = false;
		}

		private void DropItems(string itemId)
		{
			var dropData = CsvMgr.GetData<DropData>(itemId);
			Assert.IsNotNull(dropData);
			
			//赚钱和经验
			CEventCenter.BroadMessage(Message.M_PlayerExpChange,dropData.reiki*GameSettings.Instance.reikiScale);
			CEventCenter.BroadMessage(Message.M_MoneyChange, dropData.money * GameSettings.Instance.moneyScale);
			
//			Debug.Log($"掉落经验：【{dropData.reiki}】*[{GameSettings.Instance.reikiScale}]");
			//掉落物品
			ItemData itemData = null;

			#region 获取itemData

			var allRate = dropData.drag_1 + dropData.drag_2 + dropData.stone_1 + dropData.stone_2;
			var rates = new[]
			{
				dropData.drag_1 / allRate,
				(dropData.drag_1 + dropData.drag_2) / allRate,
				(dropData.drag_1 + dropData.drag_2 + dropData.stone_1) / allRate,
				(dropData.drag_1 + dropData.drag_2 + dropData.stone_1 + dropData.stone_2) / allRate,
			};
			var randomRate = Random.Range(0, 1f);
			if (randomRate > rates[3])
			{
				throw new Exception("异常概率："+randomRate);
			}else if (randomRate > rates[2])
			{
				itemData = CsvMgr.GetRandomData<GemData>(FileName.GemData_2);
			}else if (randomRate > rates[1])
			{
				itemData = CsvMgr.GetRandomData<GemData>(FileName.GemData_1);
			}else if (randomRate > rates[0])
			{
				itemData = CsvMgr.GetRandomData<DragData>(FileName.DragData_2);
			}else
			{
				itemData = CsvMgr.GetRandomData<DragData>(FileName.DragData_1);
			}			

			#endregion

			Assert.IsNotNull(itemData);
			
			var obj = ResourceMgr.InstantiateGameObject(PrefabName.DropItem, this.position);
			
			Assert.IsTrue(obj);
			
			obj.GetComponent<SpriteRenderer>().sprite =
				ResourceMgr.GetAsset<Sprite>(itemData.spriteName);

			var ti = obj.GetOrAddComponent<TriggerInputer>();
			ti.onCollisionEnterEvent2D +=
				col =>
				{
					if (col.gameObject.GetPersonInfo() is IHighFiveCharacter)
					{
						CEventCenter.BroadMessage(Message.M_AddItem, itemData.ID, 1);
						GameObject.Destroy(ti.gameObject);
					}
				};
		}
	}
}
