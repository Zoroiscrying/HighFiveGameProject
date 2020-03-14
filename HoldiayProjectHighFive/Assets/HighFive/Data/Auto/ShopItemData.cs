namespace HighFive.Data
{
	public class ShopItemData : ItemData
	{
		public const int ShopItemDataCount = 2;

		public override string ID => itemName.ToString();

		public int price;
		public int outPrice;
		public override string ToString()
		{
			var ans="==《	ShopItemData	》==\n" +
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

