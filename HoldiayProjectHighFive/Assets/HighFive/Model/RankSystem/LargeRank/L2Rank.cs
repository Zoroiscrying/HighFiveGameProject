using HighFive.Const;

namespace HighFive.Model.RankSystem.LargeRank
{
	
	public class L2Rank : AbstractLargeRank
	{

		public override bool BreakLimit()
		{
			return false;
		}

		public override void ImprovePlayer()
		{
			
		}


		public override string Sign
		{
			get { return TxtSign.L2Rank; }
		}
	}
	
}
