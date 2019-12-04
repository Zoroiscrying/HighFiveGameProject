using ReadyGamerOne.View;
using HighFive.Const;
namespace HighFive.View
{
	public partial class Shop : AbstractPanel
	{
		partial void OnLoad();

		protected override void Load()
		{
			Create(PanelPath.Shop);
			OnLoad();		}
	}
}
