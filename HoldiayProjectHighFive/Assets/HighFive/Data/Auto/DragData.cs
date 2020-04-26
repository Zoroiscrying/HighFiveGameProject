namespace HighFive.Data
{
	public class DragData : ItemData
	{
		public const int DragDataCount = 2;

		public override string ToString()
		{
			var ans="==《	DragData	》==\n" +
					"itemName" + "	" + itemName+"\n" +
					"uitext" + "	" + uitext+"\n" +
					"spriteName" + "	" + spriteName+"\n" +
					"prefabName" + "	" + prefabName+"\n" +
					"statement" + "	" + statement+"\n" +
					"maxSlotCount" + "	" + maxSlotCount+"\n" +
					"price" + "	" + price+"\n" +
					"outPrice" + "	" + outPrice;
			return ans;

		}

	}
}

