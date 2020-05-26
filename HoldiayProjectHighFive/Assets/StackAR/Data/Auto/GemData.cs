using UnityEngine;
using System.Collections;

namespace StackAR.Data
{
	public class GemData : ItemData
	{
		public const int GemDataCount = 2;

		public override string ID => itemName.ToString();

		public float attackAdder;
		public float attackSpeedAdder;
		public float repluseScaleAdder;
		public float zattackAdder;
		public float zrepluseScaleAdder;
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
					"attackAdder" + "	" + attackAdder+"\n" +
					"attackSpeedAdder" + "	" + attackSpeedAdder+"\n" +
					"repluseScaleAdder" + "	" + repluseScaleAdder+"\n" +
					"zattackAdder" + "	" + zattackAdder+"\n" +
					"zrepluseScaleAdder" + "	" + zrepluseScaleAdder+"\n" +
					"story" + "	" + story;
			return ans;

		}

	}
}

