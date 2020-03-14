using ReadyGamerOne.Rougelike.Person;
using System;
using Game.Scripts;
using HighFive.Const;
using HighFive.Data;
using ReadyGamerOne.Common;
using ReadyGamerOne.Data;
using ReadyGamerOne.Scripts;
using UnityEngine;
using UnityEngine.Assertions;
using ReadyGamerOne.Utility;

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
		
		public override Type DataType => typeof(EnemyData);

		public override void LoadData(CsvMgr data)
		{
			base.LoadData(data);
			var enemyData = CsvData as EnemyData;
			if (null == enemyData)
			{
				throw new Exception("获取敌方Data为空");
			}

			this.Attack = enemyData.attack;
			this.MaxHp = enemyData.maxHp;
			this.Hp = this.MaxHp;
		}

		public HeadUiCanvas HeadUi => (Controller as HighFiveEnemyController).HeadUi;
		
		public override void OnInstanciateObject()
		{
			base.OnInstanciateObject();
			Assert.IsTrue(HeadUi);
			headBloodBar = HeadUi.GetComponent<SuperBloodBar>("SuperBloodBar");
			Assert.IsTrue(headBloodBar);
		}
		public override void EnableObject()
		{
			base.EnableObject();
			headBloodBar.InitValue(MaxHp);
		}		
		
		
		public override void OnGetFromPool()
		{
			base.OnGetFromPool();
			HeadUi.GetComponent<Canvas>().enabled = true;
		}

		public override void OnTakeDamage(AbstractPerson takeDamageFrom, int damage)
		{
			base.OnTakeDamage(takeDamageFrom, damage);
			headBloodBar.Value -= damage;
			
			//赚钱和经验
			CEventCenter.BroadMessage(Message.M_PlayerExpChange,damage*5);
			CEventCenter.BroadMessage(Message.M_MoneyChange, damage * 3);
		}
		
		

		public override void Kill()
		{
//			Debug.Log(" 1 ");
			base.Kill();
			HeadUi.GetComponent<Canvas>().enabled = false;
			
			
		}
	}
}
