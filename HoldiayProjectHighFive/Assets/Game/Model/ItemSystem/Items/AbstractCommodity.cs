using System;
using ReadyGamerOne.Data;
using UnityEngine.Assertions;

namespace Game.Model.ItemSystem
{
	/// <summary>
	/// 商品，多了价格属性
	/// </summary>
	public abstract class AbstractCommodity : AbstractItem
	{
		public int price { get; private set; }
		protected override int BasePropertyCount
		{
			get { return base.BasePropertyCount+1; }
		}

		internal override void Init(string args)
		{
			var strs = args.Split(TxtManager.SplitChar);
			Assert.IsTrue(strs.Length>=BasePropertyCount);
			this.price = Convert.ToInt32(strs[base.BasePropertyCount]);
			base.Init(string.Join(TxtManager.SplitChar.ToString(),strs,0,base.BasePropertyCount));
		}
	}

}

