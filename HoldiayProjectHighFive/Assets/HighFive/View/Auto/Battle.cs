using ReadyGamerOne.View;
using HighFive.Const;
namespace HighFive.View
{
	public partial class Battle : AbstractPanel
	{
		partial void OnLoad();

		protected override void Load()
		{
			Create(PanelName.Battle);
			OnLoad();
		}
	}
}
