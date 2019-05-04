using System.Collections;
using System.Collections.Generic;
using Game.Const;
using UnityEngine;

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
			get { return DataSign.S2Rank; }
		}
	}
}

