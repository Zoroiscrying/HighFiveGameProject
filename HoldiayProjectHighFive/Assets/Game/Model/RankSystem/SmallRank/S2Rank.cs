using Game.Const;

namespace Game.Model.RankSystem
{
	public class S2Rank : AbstractSmallRank
	{
		public override void ImprovePlayer()
		{
			throw new System.NotImplementedException();
		}

		public override string Sign
		{
			get { return TxtSign.S2Rank; }
		}
	}
}

