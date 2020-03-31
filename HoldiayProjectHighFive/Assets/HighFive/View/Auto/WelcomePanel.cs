using ReadyGamerOne.View;
using HighFive.Const;
namespace HighFive.View
{
	public partial class WelcomePanel : AbstractPanel
	{
		partial void OnLoad();

		protected override void Load()
		{
			Create(PanelName.WelcomePanel);
			OnLoad();
		}
	}
}
