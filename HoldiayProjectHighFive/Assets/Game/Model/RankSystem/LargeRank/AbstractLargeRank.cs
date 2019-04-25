using System.Collections;
using System.Collections.Generic;
using System.IO;
using Game.Data;
using UnityEngine;
using UnityEngine.Assertions;

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
		public abstract string Sign { get; }

		public void SignInit(string initLine)
		{
			var strs = initLine.Split('|');
			Assert.IsTrue(strs.Length >= 2);
			this.name = strs[1].Trim();
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

