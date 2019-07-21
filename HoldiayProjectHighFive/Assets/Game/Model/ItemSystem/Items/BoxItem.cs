using System.Collections;
using System.Collections.Generic;
using Game.Const;
using UnityEngine;

namespace Game.Model.ItemSystem
{
	public class BoxItem:AbstractCommodity 
	{
		public override string Sign
		{
			get { return TxtSign.boxItem; }
		}
	}
}

