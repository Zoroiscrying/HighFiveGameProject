using ReadyGamerOne.Data;
using UnityEngine;
using System.Collections;

namespace HighFive.Data
{
	public class PersonData : CsvMgr
	{
		public const int PersonDataCount = 0;

		public override string ID => personName.ToString();

		public string personName;
		public float hitback_x;
		public float hitback_y;
		public override string ToString()
		{
			var ans="==《	PersonData	》==\n" +
					"personName" + "	" + personName+"\n" +
					"hitback_x" + "	" + hitback_x+"\n" +
					"hitback_y" + "	" + hitback_y;
			return ans;

		}

	}
}

