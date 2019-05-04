using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using behaviac;
using Game.Data;
using UnityEngine;
using UnityEngine.Assertions;
using Debug = behaviac.Debug;

namespace Game.Model.RankSystem
{
	public abstract class AbstractSmallRank :ITxtSerializable
	{
		public string name;
		public int max;
		public virtual int BasePropertyCount
		{
			get
			{
				return 2;
			}
		}
		public abstract void ImprovePlayer();


		public virtual void LoadTxt(string args)
		{
			var strs = args.Split(TxtManager.SplitChar);
			Assert.IsTrue(strs.Length == BasePropertyCount);
			this.name = strs[0].Trim();
			this.max = Convert.ToInt32(strs[1].Trim());
		}

		
		#region ITxtSerializable
		public abstract string Sign { get; }
		public void SignInit(string initLine)
		{
		}

		public void LoadTxt(StreamReader sr)
		{
			var line = TxtManager.ReadUntilValue(sr);
			if (string.IsNullOrEmpty(line))
			{
				Debug.LogWarning("rankinfo is null");
				return;
			}
			LoadTxt(line);
		}
		
		#endregion
	}
}
	
