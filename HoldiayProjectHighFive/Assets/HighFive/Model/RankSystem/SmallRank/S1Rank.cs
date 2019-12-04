using HighFive.Const;
using ReadyGamerOne.Data;
using UnityEngine.Assertions;

namespace HighFive.Model.RankSystem.SmallRank
{
	public class S1Rank : AbstractSmallRank
	{
		public override void ImprovePlayer()
		{
//			GlobalVar.G_Player.HitBackSpeed += this.hitback;
		}

		public override void LoadTxt(string args)
		{
			var strs = args.Split(TxtManager.SplitChar);
			Assert.IsTrue(strs.Length >= BasePropertyCount + 1);
			base.LoadTxt(string.Join(TxtManager.SplitChar.ToString(),strs,0,BasePropertyCount));
		}

		public override string Sign
		{
			get { return TxtSign.S1Rank; }
		}
	}
}

