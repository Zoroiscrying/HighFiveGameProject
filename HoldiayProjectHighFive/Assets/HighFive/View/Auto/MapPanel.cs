using ReadyGamerOne.View;
using HighFive.Const;
namespace HighFive.View
{
	public partial class MapPanel : AbstractPanel
	{
		partial void OnLoad();

		protected override void Load()
		{
			Create(PanelName.MapPanel);
			OnLoad();
		}
	}
}
