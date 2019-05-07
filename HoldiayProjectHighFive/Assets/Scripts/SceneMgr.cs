using System.Collections;
using System.Collections.Generic;
using Game;
using Game.Common;
using Game.Const;
using Game.Control;
using Game.Global;
using Game.Script;
using Game.Data;
using Game.View;
using UnityEngine;
using Mono.Data.Sqlite;
using UnityEditor;
using UnityEngine.SceneManagement;
using Game.Model;
using Game.Control.PersonSystem;
using Game.Control.SkillSystem;
using Game.Model.SpiritItemSystem;
using Game.View.PanelSystem;
using Game.Model.ItemSystem;
using Game.Model;
using Game.Model.RankSystem;
using Game.Model.SpriteObjSystem;
using UnityEngine.Assertions;
using UnityEngine.UI;

public class SceneMgr : BaseSceneMgr
{
	List<AbstractPerson> list=new List<AbstractPerson>();
	//private Player player;
	public string UiPanelName;
	public bool creatTestPeople = false;
	public Vector3 EnemyPos;
    public Vector3 PlayerPos;
    public float signalSize=4;
    private GameObject miniMap;
    
    
    #region MonoBehavior
    
    
	void Start()
	{
	//if(creatTestPeople)	
    //    CreateTestPeople();
		this.miniMap = Resources.Load<GameObject>(UIPath.Image_MiniMap);
		Assert.IsTrue(this.miniMap);
		GameObject.Instantiate(miniMap, GlobalVar.G_Canvas.transform);
		if(!string.IsNullOrEmpty(UiPanelName))
			UIManager.Instance.PushPanel(UiPanelName);
		if (creatTestPeople)
			new TestPerson("Test", GameObjectPath.TestPersonPath, EnemyPos);
		
		
		CEventCenter.BroadMessage(Message.M_RankAwake,0,0);
	}
    // Use this for initialization

    private void OnDrawGizmos()
    {
		DrawV3(PlayerPos,Color.green);
		if(creatTestPeople)
			DrawV3(EnemyPos,Color.red);
    }
    
    void Update()
	{
		if (Input.GetKeyDown(KeyCode.UpArrow))
		{
			Debug.Log("试图添加灵器" + SpiritName.C_First);
			GlobalVar.G_Player.AddSpirit(SpiritName.C_First);
			
		}

		if (Input.GetKeyDown(KeyCode.DownArrow))
		{
			Debug.Log("试图移出灵器" + SpiritName.C_First);
			GlobalVar.G_Player.RemoveSpirit(SpiritName.C_First);
		}
     			
		
		if (Input.GetKeyDown(KeyCode.K))
		{
			foreach (var t in list)
				if(t.obj!=null)
					new DirectLineBullet(10, Vector3.right*t.Dir, t.Pos+new Vector3(0,0.3f,0), t,BulletPath.PlayerBullet);
		}


		if (Input.GetKeyDown(KeyCode.S))
		{
			XmlManager.SaveData(GlobalVar.G_Player,DefaultData.PlayerDataFilePath);
			AssetDatabase.Refresh();
		}
	}	
    
    
    #endregion
	
    void DrawV3(Vector3 pos,Color color)
    {
	    Debug.DrawLine(pos + Vector3.left * this.signalSize, pos + Vector3.right * this.signalSize,color);
	    Debug.DrawLine(pos + Vector3.up * this.signalSize, pos + Vector3.down * this.signalSize, color); 
    }

	void CreateTestPeople()
	{
		var go = new GameObject("TestPeople");
		for (int i = 5; i > 0; i--)
		{
			var x = new TestPerson("TestPerson", GameObjectPath.TestPersonPath,
				new Vector3(-25 + Random.Range(0, 20), 1.28f , -1), new List<string>(new []{"O_Skill","Shot","L_Skill"}), go.transform);
			list.Add(x);
		}
	}

	#region 全局初始化
	
	#region Register
	protected override void Register()
	{
		base.Register();
		RegisterData();
		RegisterUiPanels();
		RegisterSpiritItem();
        LoadDataFromFile();

    }

	/// <summary>
	/// 注册UI面板
	/// </summary>
	void RegisterUiPanels()
	{
		AbstractPanel.RegisterPanel<BattlePanel>(PanelName.battlePanel);
        AbstractPanel.RegisterPanel<PackagePanel>(PanelName.packagePanel);
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
		TxtManager.RegisterDataFactory<ShitItem>(DataSign.shitItem);
		
		TxtManager.RegisterDataFactory<SkillInstance>(DataSign.skill);

		TxtManager.RegisterDataFactory<L1Rank>(DataSign.L1Rank);
		TxtManager.RegisterDataFactory<L2Rank>(DataSign.L2Rank);
		TxtManager.RegisterDataFactory<S1Rank>(DataSign.S1Rank);
		TxtManager.RegisterDataFactory<S2Rank>(DataSign.S2Rank);
		
		TxtManager.RegisterDataFactory<AnimationTrigger>(DataSign.animation);
		TxtManager.RegisterDataFactory<InstantRayDamageTrigger>(DataSign.instantDamage);
		TxtManager.RegisterDataFactory<AudioTrigger>(DataSign.audio);
		TxtManager.RegisterDataFactory<DashTrigger>(DataSign.dash);
		TxtManager.RegisterDataFactory<DirectLineBulletTrigger>(DataSign.bullet);
		TxtManager.RegisterDataFactory<SwordTrigger>(DataSign.trigger2D);
		TxtManager.RegisterDataFactory<ParabloaBulletTrigger>(DataSign.paraBullet);

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

		foreach (var v in RankMgr.LargeRankList)
		{
			Debug.Log(v.name);
		}
		
    }
    
    #endregion
    
    #region Initializer


	protected override void Initializer()
	{
		base.Initializer();
		InitializeSceneScripts();
		InitializeGlobalVar(this.PlayerPos);
		InitializeBehavic();
	}

	/// <summary>
	/// 初始化全局脚本
	/// </summary>
	void InitializeSceneScripts()
	{
		
		this.gameObject.AddComponent<MainLoop>();
		this.gameObject.AddComponent<AudioMgr>();
	}

	/// <summary>
	/// 初始化全局变量
	/// </summary>
	void InitializeGlobalVar(Vector3  pos)
	{
        if (pos != Vector3.zero)
            GlobalVar.Refresh(pos);
        else
            GlobalVar.Refresh();
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
	
	#endregion
	

}
