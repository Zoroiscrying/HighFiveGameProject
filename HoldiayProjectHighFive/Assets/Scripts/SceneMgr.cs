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
using Game.Control.Person;
using Game.Control.SkillSystem;
using Game.Model.SpiritItemSystem;
using Game.View.Panels;
using Game.Model.ItemSystem;
using Game.Model.SpriteObjSystem;

public class SceneMgr : BaseSceneMgr
{

	List<AbstractPerson> list=new List<AbstractPerson>();
	//private Player player;
	public string UiPanelName;
	public bool creatTestPeople = false;

	void Start()
	{
	if(creatTestPeople)	
        CreateTestPeople();
	}
	// Use this for initialization
	void Update()
	{
		if (Input.GetKeyDown(KeyCode.UpArrow))
		{
			Debug.Log("试图添加灵器" + SpiritName.C_First);
			GlobalVar.Player.AddSpirit(SpiritName.C_First);
			
		}

		if (Input.GetKeyDown(KeyCode.DownArrow))
		{
			Debug.Log("试图移出灵器" + SpiritName.C_First);
			GlobalVar.Player.RemoveSpirit(SpiritName.C_First);
		}
     			
		
		if (Input.GetKeyDown(KeyCode.K))
		{
			foreach (var t in list)
				if(t.obj!=null)
					new DirectLineBullet(10, Vector3.right*t.Dir, t.Pos+new Vector3(0,0.3f,0), t,BulletPath.PlayerBullet);
		}


		if (Input.GetKeyDown(KeyCode.S))
		{
			XmlManager.SaveData(GlobalVar.Player,DefaultData.PlayerDataFilePath);
			AssetDatabase.Refresh();
		}
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
	protected override void Register()
	{
		base.Register();
		RegisterSkillTrigger();
		RegisterUiPanels();
		RegisterSpiritItem();
        RegisterItem();

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
	}

	/// <summary>
	/// 注册技能
	/// </summary>
	void RegisterSkillTrigger()
	{
		//所有触发器种类的注册
		SkillTriggerMgr.RegisterTriggerFactory(SkillTriggerName.animation,
			SkillTriggerFactory<AnimationTrigger>.Instance);
		SkillTriggerMgr.RegisterTriggerFactory(SkillTriggerName.instantDamage,
			SkillTriggerFactory<InstantRayDamageTrigger>.Instance);
		SkillTriggerMgr.RegisterTriggerFactory(SkillTriggerName.audio,
			SkillTriggerFactory<AudioTrigger>.Instance);
		SkillTriggerMgr.RegisterTriggerFactory(SkillTriggerName.dash,
			SkillTriggerFactory<DashTrigger>.Instance);
		SkillTriggerMgr.RegisterTriggerFactory(SkillTriggerName.bullet,
			SkillTriggerFactory<DirectLineBulletTrigger>.Instance);
		SkillTriggerMgr.RegisterTriggerFactory(SkillTriggerName.trigger2D,
			SkillTriggerFactory<SwordTrigger>.Instance);
        SkillTriggerMgr.RegisterTriggerFactory(SkillTriggerName.paraBullet,
            SkillTriggerFactory<ParabloaBulletTrigger>.Instance);

		//读取文件，获取所有技能
		SkillTriggerMgr.LoadSkillsFromFile(FilePath.SkillFilePath);
	}

    void RegisterItem()
    {
        ItemMgr.RegisterItemFactory(ItemID.ShitId, ItemFactory<Shit>.Instance);

        ItemMgr.LoadItemsFromFile(FilePath.ItemFilePath);
    }


	protected override void Initializer()
	{
		base.Initializer();
		InitializeSceneScripts();
		InitializeGlobalVar();
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
	void InitializeGlobalVar()
	{
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
	

}
