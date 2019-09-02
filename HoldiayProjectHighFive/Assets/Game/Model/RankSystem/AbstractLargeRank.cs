using System;
using System.Collections.Generic;
using System.IO;
using ReadyGamerOne.Data;

namespace Game.Model.RankSystem
{
	public abstract class AbstractLargeRank:ITxtSerializable
	{
		public string name;
		public List<AbstractSmallRank> smallRanks = new List<AbstractSmallRank>();
		public virtual int BasePropertyCount
		{
			get
			{
				return 1;
			}
		}
		public abstract bool BreakLimit();
		public abstract void ImprovePlayer();

		public virtual void LoadTxt(string args)
		{
			var strs = args.Split(TxtManager.SplitChar);
			if (strs.Length < BasePropertyCount)
			{
				throw new Exception(args+"\n"+"AbstractLargeRank.strs.Length: "+strs.Length);
			}
			//Assert.IsTrue(strs.Length==BasePropertyCount);
			this.name = strs[0].Trim();
		}

		public abstract string Sign { get; }

		public void SignInit(string initLine)
		{
			var strs = initLine.Split(TxtManager.SplitChar);
//			Debug.Log("L1Rank strs.Length: "+strs.Length);
			LoadTxt(string.Join(TxtManager.SplitChar.ToString(), strs, 1,strs.Length-1));
		}

		public void LoadTxt(StreamReader sr)
		{
			do
			{
				var smallRank = TxtManager.LoadData(sr) as AbstractSmallRank;
				//throw new Exception(sr.ReadLine());
				if (smallRank == null)
				{
//                    Debug.LogWarning("trigger is null");
					break;
				}

				this.smallRanks.Add(smallRank);
			} while (true);
		}
	}
}

