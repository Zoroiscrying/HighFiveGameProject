using ReadyGamerOne.View;
using HighFive.Const;
namespace HighFive.View
{
	public partial class Loading : AbstractPanel
	{
		partial void OnLoad();

		protected override void Load()
		{
			Create(PanelPath.Loading);
			OnLoad();		}
	}
}
