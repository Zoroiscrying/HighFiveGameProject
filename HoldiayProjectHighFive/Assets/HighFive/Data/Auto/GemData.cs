using UnityEngine;
using System.Collections;

namespace HighFive.Data
{
	public class GemData : ItemData
	{
		public const int GemDataCount = 3;

		public override string ID => itemName.ToString();

		public int attack;
		public float attackSpeed;
		public override string ToString()
		{
			var ans="==《	GemData	》==\n" +
					"itemName" + "	" + itemName+"\n" +
					"uitext" + "	" + uitext+"\n" +
					"spriteName" + "	" + spriteName+"\n" +
					"prefabName" + "	" + prefabName+"\n" +
					"statement" + "	" + statement+"\n" +
					"maxSlotCount" + "	" + maxSlotCount+"\n" +
					"price" + "	" + price+"\n" +
					"outPrice" + "	" + outPrice+"\n" +
					"attack" + "	" + attack+"\n" +
					"attackSpeed" + "	" + attackSpeed;
			return ans;

		}

	}
}

