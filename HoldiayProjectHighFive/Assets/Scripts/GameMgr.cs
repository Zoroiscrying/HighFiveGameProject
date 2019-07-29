using Game.Common;
using Game.Const;
using Game.Control.SkillSystem;
using Game.Data;
using Game.Global;
using Game.Model.ItemSystem;
using Game.Model.RankSystem;
using Game.Model.SceneSystem;
using Game.Model.SpiritItemSystem;
using Game.Script;
using Game.View.PanelSystem;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameMgr : MonoSingleton<GameMgr> {
	
	[HideInInspector]
	public Vector3 PlayerPos;
	
	protected override void Awake()
	{
		if (GameMgr.Instance&&this != GameMgr.Instance)
		{
			this.gameObject.SetActive(false);
			return;
		}
		
		base.Awake();
		
		print("GameMgr_Awake_这句话应该只打印一次才对");

		AbstractSceneInfo.onAnySceneLoad += this.OnAnySceneLoad;
		AbstractSceneInfo.onAnySceneUnLoaded += this.OnAnySceneUnLoad;
		
		
		
		PanelMgr.onClear += () =>
		{
			for (int i = 0; i <transform.childCount; i++)
			{
				var child = transform.GetChild(i);
				Destroy(child.gameObject);
			}
		};
		
		RegisterAll();
		
		AddGlobalScript();

		InitializeBehavic();
		
		
	}

	// Use this for initialization
	void Start ()
	{
		SceneMgr.LoadActiveScene();
		print("GameMgr_Start_这句话也应该只打印一次才对");
		DontDestroyOnLoad(this.gameObject);
		
	}

	private void OnAnySceneLoad()
	{
		Debug.Log("GameMgr_OnAnySceneLoad");
		InitializeGlobalVar(this.PlayerPos);
	}

	private void OnAnySceneUnLoad()
	{
		PanelMgr.Clear();
		MainLoop.Instance.Clear();
		CEventCenter.Clear();
	}
	#region 全局初始化
	
	#region Register
	
	public void RegisterAll()
	{
		RegisterData();
        RegisterUiPanels();
        RegisterSpiritItem();
        RegisterSceneName();
        LoadDataFromFile();
	}

	/// <summary>
	/// 注册UI面板
	/// </summary>
	void RegisterUiPanels()
	{
		AbstractPanel.RegisterPanel<BattlePanel>(PanelName.battlePanel);
        AbstractPanel.RegisterPanel<PackagePanel>(PanelName.packagePanel);
        AbstractPanel.RegisterPanel<ShopPanel>(PanelName.shopPanel);
        AbstractPanel.RegisterPanel<LoadingPanel>(PanelName.loadingPanel);
	}

	void RegisterSceneName()
	{
		AbstractSceneInfo.RegisterSceneInfo<JbSceneInfo>(SceneName.jbScene);
		AbstractSceneInfo.RegisterSceneInfo<TestSceneInfo>(SceneName.testScene);
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
		TxtManager.RegisterDataFactory<ShitItem>(TxtSign.shitItem);
		TxtManager.RegisterDataFactory<BoxItem>(TxtSign.boxItem);
		
		TxtManager.RegisterDataFactory<SkillInstance>(TxtSign.skill);

		TxtManager.RegisterDataFactory<L1Rank>(TxtSign.L1Rank);
		TxtManager.RegisterDataFactory<L2Rank>(TxtSign.L2Rank);
		TxtManager.RegisterDataFactory<S1Rank>(TxtSign.S1Rank);
		TxtManager.RegisterDataFactory<S2Rank>(TxtSign.S2Rank);
		
		TxtManager.RegisterDataFactory<AnimationTrigger>(TxtSign.animation);
		TxtManager.RegisterDataFactory<InstantRayDamageTrigger>(TxtSign.instantDamage);
		TxtManager.RegisterDataFactory<AudioTrigger>(TxtSign.audio);
		TxtManager.RegisterDataFactory<DashTrigger>(TxtSign.dash);
		TxtManager.RegisterDataFactory<DirectLineBulletTrigger>(TxtSign.bullet);
		TxtManager.RegisterDataFactory<SwordTrigger>(TxtSign.trigger2D);
		TxtManager.RegisterDataFactory<ParabloaBulletTrigger>(TxtSign.paraBullet);

	}

	/// <summary>
	/// 从文件中加载数据
	/// </summary>
    void LoadDataFromFile()
    {
	    TxtManager.LoadDataFromFile<AbstractLargeRank>(FilePath.RankFilePath,
		    (largeRank) => { RankMgr.LargeRankList.Add(largeRank); });
        
		TxtManager.LoadDataFromFile<SkillInstance>(FilePath.SkillFilePath,
			(skillInstance) => { SkillTriggerMgr.skillInstanceDic.Add(skillInstance.name, skillInstance); });
        
		TxtManager.LoadDataFromFile<AbstractItem>(FilePath.ItemFilePath,
			(item) =>{ ItemMgr.itemDic.Add(item.ID, item); });
//
//		foreach (var v in RankMgr.LargeRankList)
//		{
//			Debug.Log(v.name);
//		}
		
    }
	
    /// <summary>
    /// 初始化全局脚本
    /// </summary>
    void AddGlobalScript()
    {
    	this.gameObject.AddComponent<MainLoop>();
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
	void InitializeGlobalVar(Vector3  pos)
	{
//		Debug.Log("GameMgr_RefreshGlobalVar: "+pos);
        if (pos != Vector3.zero)
            GlobalVar.Refresh(pos);
        else
            GlobalVar.Refresh();
	}
	

	
	#endregion
	
	#endregion
}
