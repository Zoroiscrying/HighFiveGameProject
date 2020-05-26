using UnityEngine;
using System.Collections;

namespace StackAR.Data
{
	public class BossData : EnemyData
	{
		public const int BossDataCount = 1;

		public override string ToString()
		{
			var ans="==《	BossData	》==\n" +
					"personName" + "	" + personName+"\n" +
					"hitback_x" + "	" + hitback_x+"\n" +
					"hitback_y" + "	" + hitback_y+"\n" +
					"maxHp" + "	" + maxHp+"\n" +
					"attack" + "	" + attack;
			return ans;

		}

	}
}

