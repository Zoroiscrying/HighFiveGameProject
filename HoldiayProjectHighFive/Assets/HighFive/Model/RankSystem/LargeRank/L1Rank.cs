using HighFive.Const;
using ReadyGamerOne.Data;
using UnityEngine.Assertions;

namespace HighFive.Model.RankSystem.LargeRank
{
	public class L1Rank : AbstractLargeRank
	{

		public override bool BreakLimit()
		{
			return true;
		}

		public override void LoadTxt(string args)
		{
			var strs = args.Split(TxtManager.SplitChar);
//			Debug.Log("L1Rank.strs.Length: "+strs.Length);
			Assert.IsTrue(strs.Length>=BasePropertyCount+1);
			base.LoadTxt(string.Join(TxtManager.SplitChar.ToString(),strs,0,BasePropertyCount));
		}

		public override void ImprovePlayer()
		{
//			GlobalVar.G_Player.playerInfo.attack_scaler *= this.attackMut;
		}

		public override string Sign
		{
			get { return TxtSign.L1Rank; }
		}
	}
}

