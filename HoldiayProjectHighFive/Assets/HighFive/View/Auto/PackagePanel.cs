using ReadyGamerOne.View;
using HighFive.Const;
namespace HighFive.View
{
	public partial class PackagePanel : AbstractPanel
	{
		partial void OnLoad();

		protected override void Load()
		{
			Create(PanelName.PackagePanel);
			OnLoad();
		}
	}
}
