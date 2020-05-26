using ReadyGamerOne.Data;
using UnityEngine;
using System.Collections;

namespace StackAR.Data
{
	public class RankData : CsvMgr
	{
		public const int RankDataCount = 3;

		public override string ID => level.ToString();

		public int level;
		public int maxHp;
		public int maxExp;
		public string rankName;
		public string limit;
		public int bloodC;
		public int attackC;
		public override string ToString()
		{
			var ans="==《	RankData	》==\n" +
					"level" + "	" + level+"\n" +
					"maxHp" + "	" + maxHp+"\n" +
					"maxExp" + "	" + maxExp+"\n" +
					"rankName" + "	" + rankName+"\n" +
					"limit" + "	" + limit+"\n" +
					"bloodC" + "	" + bloodC+"\n" +
					"attackC" + "	" + attackC;
			return ans;

		}

	}
}

