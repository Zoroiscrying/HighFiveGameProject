using ReadyGamerOne.View;
using HighFive.Const;
namespace HighFive.View
{
	public partial class Package : AbstractPanel
	{
		partial void OnLoad();

		protected override void Load()
		{
			Create(PanelPath.Package);
			OnLoad();		}
	}
}
