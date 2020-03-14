using ReadyGamerOne.Data;

namespace HighFive.Data
{
	public class ItemData : CsvMgr
	{
		public const int ItemDataCount = 6;

		public override string ID => itemName.ToString();

		public string itemName;
		public string uitext;
		public string spriteName;
		public string prefabName;
		public string statement;
		public int maxSlotCount;
		public override string ToString()
		{
			var ans="==《	ItemData	》==\n" +
					"itemName" + "	" + itemName+"\n" +
					"uitext" + "	" + uitext+"\n" +
					"spriteName" + "	" + spriteName+"\n" +
					"prefabName" + "	" + prefabName+"\n" +
					"statement" + "	" + statement+"\n" +
					"maxSlotCount" + "	" + maxSlotCount;
			return ans;

		}

	}
}

