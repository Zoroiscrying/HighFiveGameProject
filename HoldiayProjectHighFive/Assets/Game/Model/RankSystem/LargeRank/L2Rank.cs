using System.Collections;
using System.Collections.Generic;
using Game.Const;
using UnityEngine;

namespace Game.Model.RankSystem
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
