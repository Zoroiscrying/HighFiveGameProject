using HighFive.Const;
using ReadyGamerOne.Common;
using ReadyGamerOne.Model.SceneSystem;
using ReadyGamerOne.View;
using TMPro;
using UnityEngine.Assertions;
using UnityEngine.UI;

namespace HighFive.View
{
	public partial class LoadingPanel
	{      
		private Slider slider;
		private TextMeshProUGUI text;
		partial void OnLoad()
		{
			//do any thing you want
			this.slider = view["Slider_Progress"].GetComponent<Slider>();
			this.text = view["Tmp_LoadingText"].GetComponent<TextMeshProUGUI>();

			Assert.IsTrue(this.slider && this.text);
		}
		protected override void OnAddListener()
		{
			base.OnAddListener();
			CEventCenter.AddListener<string>(Message.M_LoadSceneAsync,OnLoadSceneAsync);
		}

		protected override void OnRemoveListener()
		{
			base.OnRemoveListener();
			CEventCenter.RemoveListener<string>(Message.M_LoadSceneAsync,OnLoadSceneAsync);
		}

		private void OnLoadSceneAsync(string sceneName)
		{
			if(sceneName==SceneMgr.CurrentSceneName)
				PanelMgr.PopPanel();
			SceneMgr.LoadSceneAsync(sceneName, onLoading: (value) =>
			{
				this.slider.value = (float) value / 100.0f;
				this.text.text = "LoadingPanel..." + value + "%";
			});
		}
	}
}
