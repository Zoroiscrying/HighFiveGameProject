using ReadyGamerOne.Data;
using UnityEngine;
using System.Collections;

namespace HighFive.Data
{
	public class DropData : CsvMgr
	{
		public const int DropDataCount = 2;

		public override string ID => personName.ToString();

		public string personName;
		public int money;
		public float drag_1;
		public float drag_2;
		public float stone_1;
		public float stone_2;
		public float reiki;
		public override string ToString()
		{
			var ans="==《	DropData	》==\n" +
					"personName" + "	" + personName+"\n" +
					"money" + "	" + money+"\n" +
					"drag_1" + "	" + drag_1+"\n" +
					"drag_2" + "	" + drag_2+"\n" +
					"stone_1" + "	" + stone_1+"\n" +
					"stone_2" + "	" + stone_2+"\n" +
					"reiki" + "	" + reiki;
			return ans;

		}

	}
}

