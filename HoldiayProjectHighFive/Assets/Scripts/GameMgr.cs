﻿using Game.Const;
using Game.Control.PersonSystem;
using Game.Model.ItemSystem;
using Game.Model.RankSystem;
using Game.Model.SpiritItemSystem;
using Game.View.PanelSystem;
using ReadyGamerOne.Common;
using ReadyGamerOne.Data;
using ReadyGamerOne.EditorExtension;
using ReadyGamerOne.Global;
using ReadyGamerOne.MemorySystem;
using ReadyGamerOne.Script;
using ReadyGamerOne.View.PanelSystem;
using UnityEngine;

namespace Game.Scripts
{
	public class GameMgr :AbstractGameMgr
	{
    	#region 单例
    
    	private static GameMgr instance;
    	public static GameMgr Instance
    	{
    		get { return instance; }
    		private set { instance = value; }
    	}
    	protected override void Awake()
    	{		
    		if (instance &&this != Instance)
            {
            	this.gameObject.SetActive(false);
            	return;
            }
    
    		instance = this;
    		base.Awake();



        }



    	#endregion
        
    
    	protected override void WorkForOnlyOnce()
    	{
    		base.WorkForOnlyOnce();		
    		
    		
    		print("GameMgr work for only once");
    
    
    		MemoryMgr.LoadAssetFromResourceDir<AudioClip>(typeof(AudioName), DirPath.AudioDir,
    			(name, clip) => AudioMgr.Instance.audioclips.Add(name, clip));
    		
    		PanelMgr.onClear += () =>
            {
            	for (int i = 0; i <transform.childCount; i++)
            	{
            		var child = transform.GetChild(i);
            		Destroy(child.gameObject);
            	}
            };
            
            RegisterAll();
            
            InitializeBehavic();
    	}
    
    
    
    	protected override void OnAnySceneUnload()
    	{
    		base.OnAnySceneUnload();
    		PanelMgr.Clear();
    		MainLoop.Clear();
    		CEventCenter.Clear();
    	}
    	#region 全局初始化
    	
    	#region Register
    	
    	public void RegisterAll()
    	{
    		print("GameMgr RegisterAll");
    		RegisterData();
            RegisterSpiritItem();
            LoadDataFromFile();
    	}

    
    	/// <summary>
    	/// 注册灵器
    	/// </summary>
    	void RegisterSpiritItem()
    	{
    		AbstractSpiritItem.RegisterSpiritItem<TestSpirit>(SpiritName.C_First);
    		
    		AbstractSpiritItem.RegisterSpiritItem<TestSpirit>(SpiritName.C_Second);
    		
    		
    	}
    
    	/// <summary>
    	/// 注册Data工厂
    	/// </summary>
    	void RegisterData()
    	{
    		
    
    		TxtManager.RegisterDataFactory<L1Rank>(TxtSign.L1Rank);
    		TxtManager.RegisterDataFactory<L2Rank>(TxtSign.L2Rank);
    		TxtManager.RegisterDataFactory<S1Rank>(TxtSign.S1Rank);
    		TxtManager.RegisterDataFactory<S2Rank>(TxtSign.S2Rank);
    		
    
    	}
    
    	/// <summary>
    	/// 从文件中加载数据
    	/// </summary>
        void LoadDataFromFile()
        {
    	    TxtManager.LoadDataFromFile<AbstractLargeRank>(FilePath.RankFilePath,
    		    (largeRank) => { RankMgr.LargeRankList.Add(largeRank); });
            
        }
    
    	/// <summary>
    	/// 初始化全局脚本
    	/// </summary>
    	protected override void AddGlobalScript()
    	{
    		base.AddGlobalScript();
            this.gameObject.AddComponent<AudioMgr>();
    	}
    
        /// <summary>
        /// 初始化行为树
        /// </summary>
        /// <returns></returns>
        bool InitializeBehavic()
        {
            behaviac.Workspace.Instance.FilePath = Application.dataPath + "/Scripts/behaviac/exported/behaviac_generated/types";
            behaviac.Workspace.Instance.FileFormat = behaviac.Workspace.EFileFormat.EFF_xml;
            return true;
        }
        
        #endregion
    
        #region Initializer
    
        /// <summary>
    	/// 初始化全局变量
    	/// </summary>
        protected override void RefreshGlobalVar()
        {
    	    base.RefreshGlobalVar();
    	    GlobalVar.G_Canvas = this.gameObject;
        }
    
        #endregion
    	
    	#endregion
    }

}

