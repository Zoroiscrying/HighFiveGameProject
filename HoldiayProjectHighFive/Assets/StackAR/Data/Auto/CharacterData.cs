using UnityEngine;
using System.Collections;

namespace StackAR.Data
{
	public class CharacterData : PersonData
	{
		public const int CharacterDataCount = 1;

		public override string ID => personName.ToString();

		public float defaultConstTime;
		public override string ToString()
		{
			var ans="==《	CharacterData	》==\n" +
					"personName" + "	" + personName+"\n" +
					"hitback_x" + "	" + hitback_x+"\n" +
					"hitback_y" + "	" + hitback_y+"\n" +
					"defaultConstTime" + "	" + defaultConstTime;
			return ans;

		}

	}
}

