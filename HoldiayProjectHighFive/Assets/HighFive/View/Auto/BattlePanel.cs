using ReadyGamerOne.View;
using HighFive.Const;
namespace HighFive.View
{
	public partial class BattlePanel : AbstractPanel
	{
		partial void OnLoad();

		protected override void Load()
		{
			Create(PanelName.BattlePanel);
			OnLoad();
		}
	}
}
