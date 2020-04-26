namespace HighFive.Data
{
	public class CharacterData : PersonData
	{
		public const int CharacterDataCount = 1;

		public override string ToString()
		{
			var ans="==《	CharacterData	》==\n" +
					"personName" + "	" + personName+"\n" +
					"hitback_x" + "	" + hitback_x+"\n" +
					"hitback_y" + "	" + hitback_y;
			return ans;

		}

	}
}

