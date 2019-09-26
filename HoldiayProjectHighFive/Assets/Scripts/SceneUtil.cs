using System.Collections.Generic;
using UnityEngine;
using Game.Const;
using Game.Control.PersonSystem;
using ReadyGamerOne.Common;
using ReadyGamerOne.EditorExtension;
using ReadyGamerOne.Global;
using ReadyGamerOne.MemorySystem;
using ReadyGamerOne.Model.SceneSystem;
using ReadyGamerOne.View.AssetUi;
using ReadyGamerOne.View.PanelSystem;

namespace Game.Scripts
{
    public class SceneUtil : MonoBehaviour
    {
    
        #region Private_Fields
    
            private List<AbstractPerson> list = new List<AbstractPerson>();
            private GameObject miniMap;
    
    
        #endregion
    
        #region Public_Fields

        [Header("设定当前场景UIPanel")] public PanelUiAsset PanelUiAsset;
        
        [Header("是否生成敌方假人")]
        public bool creatTestPeople = false;
        
        [Header("假人生成位置，红线十字标志")]
        public Vector3 EnemyPos;
        
        [Header("玩家生成位置，绿线十字标志")]
        public Vector3 PlayerPos;
        
        [Header("标志大小")]
        public float signalSize = 4;
        
        [Header("是否开启小地图")]
        public bool enableMiniMap = true;
    
        [Header("启用V键测试异步加载场景")]
        public bool enable_V_loadAsync = true;
        
        public StringChooser sceneName=new StringChooser(typeof(SceneName));
        
        [Header("启用C测试(伪)同步加载场景")]
        public bool Enable_C_ChangeScene = true;
        
    
        #endregion
        
        #region MonoBehavior
    
        private void Awake()
        {
            //print("Normal_Awake");
            GameMgr.Instance.PlayerPos = this.PlayerPos;
            
    //        if (GameObject.FindGameObjectsWithTag("GameController").Length <= 0)
    //            MemoryMgr.InstantiateGameObject(DirPath.GameObjectDir + GameObjectName.GameMgr);
    //        
        }
    
        void Start()
        {
            //print("Normal_Start");
    
            //Debug.Log("SceneUtil: " + PlayerPos);
            
            if (enableMiniMap)
                MemoryMgr.InstantiateGameObject(DirPath.LittleUiDir + UiName.Image_MiniMap, GlobalVar.G_Canvas.transform);


            if (PanelUiAsset != null)
                PanelAssetMgr.PushPanel(PanelUiAsset);
            CEventCenter.BroadMessage(Message.M_RankAwake,0,0);
    
//            if (creatTestPeople)
//                new TestPerson("Test", DirPath.GameObjectDir + GameObjectName.TestPerson, EnemyPos);
            
        }
        
        private void OnDrawGizmos()
        {
            DrawV3(PlayerPos, Color.green);
            if (creatTestPeople)
                DrawV3(EnemyPos, Color.red);
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
                    PanelMgr.PushPanelWithMessage(PanelName.loadingPanel, Message.M_LoadSceneAsync, sceneName.StringValue);
                }
            }
        }
    
        #endregion
        
        #region private
    
        void DrawV3(Vector3 pos, Color color)
        {
            Debug.DrawLine(pos + Vector3.left * this.signalSize, pos + Vector3.right * this.signalSize, color);
            Debug.DrawLine(pos + Vector3.up * this.signalSize, pos + Vector3.down * this.signalSize, color);
        }
    
//        void CreateTestPeople()
//        {
//            var go = new GameObject("TestPeople");
//            for (int i = 5; i > 0; i--)
//            {
//                var x = new TestPerson("TestPerson", DirPath.GameObjectDir + GameObjectName.TestPerson,
//                    new Vector3(-25 + Random.Range(0, 20), 1.28f, -1),
//                    new List<string>(new[] {"O_Skill", "Shot", "L_Skill"}), go.transform);
//                list.Add(x);
//            }
//        }
    
        #endregion
    }

}

