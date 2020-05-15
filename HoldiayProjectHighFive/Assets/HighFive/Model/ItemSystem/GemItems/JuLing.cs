using System;
using HighFive.Data;
using HighFive.Global;
using UnityEngine.Assertions;

namespace HighFive.Model.ItemSystem
{
	public partial class JuLing : HighFiveItem
	{
		protected override void AddEffects()
		{
			base.AddEffects();
			var gemdata = _itemData as GemData;
			if (null == gemdata)
				throw new Exception($"{UiText} 转化GemData异常");
			Assert.IsNotNull(GlobalVar.G_Player);
			GlobalVar.G_Player.AttackAdder += gemdata.attackAdder;
		}

		protected override void RemoveEffects()
		{
			base.RemoveEffects();
			var gemdata = _itemData as GemData;
			if (null == gemdata)
				throw new Exception($"{UiText} 转化GemData异常");
			Assert.IsNotNull(GlobalVar.G_Player);
			GlobalVar.G_Player.AttackAdder -= gemdata.attackAdder;
		}
	}
}
