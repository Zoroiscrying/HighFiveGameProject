using ReadyGamerOne.View;
using HighFive.Const;
namespace HighFive.View
{
	public partial class LoadingPanel : AbstractPanel
	{
		partial void OnLoad();

		protected override void Load()
		{
			Create(PanelName.LoadingPanel);
			OnLoad();
		}
	}
}
