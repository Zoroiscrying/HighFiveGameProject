using UnityEngine;
using HighFive.Const;
using ReadyGamerOne.Common;
using ReadyGamerOne.EditorExtension;
using HighFive.Global;
using ReadyGamerOne.MemorySystem;
using ReadyGamerOne.Model.SceneSystem;
using ReadyGamerOne.View;
using ReadyGamerOne.View.AssetUi;

namespace Game.Scripts
{
    public class SceneUtil : MonoBehaviour
    {
    
        #region Private_Fields

        #endregion
    
        #region Public_Fields

        [Header("设定当前场景UIPanel")] public PanelUiAsset PanelUiAsset;
        
        [Header("是否生成敌方假人")]
        public bool creatTestPeople = false;

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

            if (enableMiniMap)
                MemoryMgr.InstantiateGameObject(UiPath.Image_MiniMapBackGround,
                    GlobalVar.G_Canvas.transform);


            if (PanelUiAsset != null)
                PanelAssetMgr.PushPanel(PanelUiAsset);
            CEventCenter.BroadMessage(Message.M_RankAwake, 0, 0);

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
                    PanelMgr.PushPanelWithMessage(PanelName.Loading, Message.M_LoadSceneAsync, sceneName.StringValue);
                }
            }
        }
    
        #endregion
        
    }

}

