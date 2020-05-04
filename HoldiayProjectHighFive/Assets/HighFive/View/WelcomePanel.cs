using HighFive.Const;
using ReadyGamerOne.View;

namespace HighFive.View
{
	public partial class WelcomePanel
	{
		partial void OnLoad()
		{
			//do any thing you want
			add_button_listener("Btns/StartBtn",
				()=>
				PanelMgr.PushPanelWithMessage(
					PanelName.LoadingPanel, 
					Message.M_LoadSceneAsync, 
					SceneName.jbScene));
		}
	}
}
