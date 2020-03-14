using System;
using HighFive.Data;
using ReadyGamerOne.Common;
using ReadyGamerOne.Data;
using UnityEngine;

namespace HighFive.Model.RankSystem
{
	public class RankMgr:Singleton<RankMgr>
	{
		public int Level { get; private set; } = 1;
		private int _exp;
		private int _hp;
		private RankData _rankData;

		public int BaseAttack { get; private set; }
		public int Hp
		{
			get { return _hp; }
			set { _hp = value; }
		} 
		public int MaxHp => _rankData.maxHp;

		public int Exp
		{
			get { return _exp; }
			set { _exp = value; }
		}
		public int MaxExp => RankData.maxExp;
		public string Rank => RankData.rankName;
		
		private RankData RankData
		{
			get
			{
				if (null == _rankData)
				{
					_rankData = CsvMgr.GetData<RankData>(Level.ToString());
					RefreshValues();
				}
				return _rankData;
			}
		}

		public void Init(int level)
		{
			this.Level = level;
			this._rankData=CsvMgr.GetData<RankData>(Level.ToString());
			RefreshValues();
		}

		public bool TryLevelUp(int extraExp,Action<RankData> improvePlayer)
		{
			if (!BreakLimit())
				return false;

			if (Level + 1 > RankData.RankDataCount)
			{
				Debug.LogWarning("超过等级上限：" + RankData.RankDataCount);
				return false;
			}

			this._rankData=CsvMgr.GetData<RankData>((++Level).ToString());
			
			RefreshValues(extraExp);
			
			//计算增益
			improvePlayer?.Invoke(this._rankData);

			return true;
		}

		public virtual bool BreakLimit()
		{
			return true;
		}

		private void RefreshValues(int? exp=null, int? hp=null)
		{
			_hp = hp ?? MaxHp;
			_exp = exp ?? 0;
			BaseAttack = _rankData.attackC;
		}
	}
}

