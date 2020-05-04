using ReadyGamerOne.View;
using HighFive.Const;
namespace HighFive.View
{
	public partial class ShopPanel : AbstractPanel
	{
		partial void OnLoad();

		protected override void Load()
		{
			Create(PanelName.ShopPanel);
			OnLoad();
		}
	}
}
