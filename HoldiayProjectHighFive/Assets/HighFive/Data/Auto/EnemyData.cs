using UnityEngine;
using System.Collections;

namespace HighFive.Data
{
	public class EnemyData : PersonData
	{
		public const int EnemyDataCount = 2;

		public override string ID => personName.ToString();

		public int maxHp;
		public int attack;
		public override string ToString()
		{
			var ans="==《	EnemyData	》==\n" +
					"personName" + "	" + personName+"\n" +
					"hitback_x" + "	" + hitback_x+"\n" +
					"hitback_y" + "	" + hitback_y+"\n" +
					"maxHp" + "	" + maxHp+"\n" +
					"attack" + "	" + attack;
			return ans;

		}

	}
}

