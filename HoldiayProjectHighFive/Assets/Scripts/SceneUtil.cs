﻿using System;
using UnityEngine;
using HighFive.Const;
using ReadyGamerOne.EditorExtension;
using HighFive.Global;
using ReadyGamerOne.Common;
using ReadyGamerOne.MemorySystem;
using ReadyGamerOne.Model.SceneSystem;
using ReadyGamerOne.Script;
using ReadyGamerOne.View;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace HighFive.Script
{
    /// <summary>
    /// 每个Scene必备的控制整个Scene逻辑的脚本
    /// </summary>
    public class SceneUtil : MonoSingleton<SceneUtil>
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


        [Header("Boss场景需要的BossArea四个角")]
        public Transform bossLT;
        public Transform bossRT;
        public Transform bossLB;
        public Transform bossRB;


        [Header("用于控制地图")] 
        public Vector3Int mapMin;
        public Vector3Int mapMax;
        
        #endregion
        
        #region MonoBehavior

        void Start()
        {
            if (!gameObject.activeSelf || !enabled)
                return;
            PanelMgr.PushPanel(startPanel.StringValue);
            AudioMgr.Instance.PlayBgm(startBgm.StringValue);
            

            if (enableMiniMap)
                ResourceMgr.InstantiateGameObject(UiName.Image_MiniMapBackGround,
                    GlobalVar.GCanvasObj.transform);

        }


        private void Update()
        {
            if (!gameObject.activeSelf || !enabled)
                return;
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


        private void OnDrawGizmos()
        {
            if (!gameObject.activeSelf || !enabled)
                return;

            Gizmos.color=Color.magenta;
#if UNITY_EDITOR
            Handles.Label(mapMax, "MapMax");
            Handles.Label(mapMin, "MapMin");
#endif
            Gizmos.DrawWireSphere(mapMax, 0.5f);
            Gizmos.DrawWireSphere(mapMin, 0.5f);
        }

        #endregion
    }
}

