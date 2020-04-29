using UnityEngine;
using HighFive.Const;
using ReadyGamerOne.EditorExtension;
using HighFive.Global;
using ReadyGamerOne.MemorySystem;
using ReadyGamerOne.Model.SceneSystem;
using ReadyGamerOne.Script;
using ReadyGamerOne.View;

namespace Game.Scripts
{
    /// <summary>
    /// 每个Scene必备的控制整个Scene逻辑的脚本
    /// </summary>
    public class SceneUtil : MonoBehaviour
    {
        #region Public_Fields

       // [Header("设定当前场景UIPanel")] public PanelUiAsset PanelUiAsset;
        
//        [Header("是否生成敌方假人")]
//        public bool creatTestPeople = false;

        [Header("当前场景初始Panel")]
        public StringChooser startPanel=new StringChooser(typeof(PanelName));
        
        [Header("当前场景初始Bgm")]
        public StringChooser startBgm=new StringChooser(typeof(AudioName));

        [Header("是否开启小地图")]
        public bool enableMiniMap = true;
    
        [Header("启用V键测试异步加载场景")]
        public bool enable_V_loadAsync = true;
        
        public StringChooser sceneName=new StringChooser(typeof(SceneName));
        
        [Header("启用C测试(伪)同步加载场景")]
        public bool Enable_C_ChangeScene = true;
        
        #endregion
        
        #region MonoBehavior

        void Start()
        {
            PanelMgr.PushPanel(startPanel.StringValue);
            AudioMgr.Instance.PlayBgm(startBgm.StringValue);
            

            if (enableMiniMap)
                ResourceMgr.InstantiateGameObject(UiName.Image_MiniMapBackGround,
                    GlobalVar.GCanvasObj.transform);

        }


        private void Update()
        {
            if (Enable_C_ChangeScene)
            {
                if (Input.GetKeyDown(KeyCode.C))
                    SceneMgr.LoadScene(sceneName.StringValue, null, () => Debug.Log("Enter new Scene"));
            }
    
            if (enable_V_loadAsync)
            {
                if (Input.GetKeyDown(KeyCode.V))
                {
                    PanelMgr.PushPanelWithMessage(PanelName.LoadingPanel, Message.M_LoadSceneAsync, sceneName.StringValue);
                }
            }
        }
    
        #endregion
        
    }

}

