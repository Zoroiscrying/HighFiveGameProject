using Game.Const;

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

