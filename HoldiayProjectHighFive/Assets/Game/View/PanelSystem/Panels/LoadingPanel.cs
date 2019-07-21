using Game.Common;
using Game.Const;
using Game.Model.SceneSystem;
using TMPro;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;

namespace Game.View.PanelSystem
{
    public class LoadingPanel:AbstractPanel
    {
        private Slider slider;
        private TextMeshProUGUI text;
        protected override void Load()
        {
            Create(DirPath.PanelDir + Const.PanelName.loadingPanel);

            this.slider = m_TransFrom.Find("Slider_Progress").GetComponent<Slider>();
            this.text = m_TransFrom.Find("Tmp_LoadingText").GetComponent<TextMeshProUGUI>();

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
                this.text.text = "Loading..." + value + "%";
            });
        }
    }
}