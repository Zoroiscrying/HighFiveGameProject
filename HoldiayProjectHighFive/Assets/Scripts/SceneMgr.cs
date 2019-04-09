using System.Collections;
using System.Collections.Generic;
using Game;
using Game.Const;
using Game.Control;
using Game.Global;
using Game.Script;
using Game.Serialization;
using Game.View;
using UnityEngine;
using Mono.Data.Sqlite;
using UnityEditor;
using UnityEngine.SceneManagement;
using Game.Model;
using Game.Control.Person;
using Game.Const;
using Game.Control.SkillSystem;
using Game.Model.SpiritItems;
using Game.View.Panels;

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
					new Bullet(10, t.Dir, t.Pos+new Vector3(0,0.3f,0), t);
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
				new Vector3(-25 + Random.Range(0, 20), 1.28f , -1), new List<string>(new []{"O_Skill","H_Skill","L_Skill"}), go.transform);
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
	}

	/// <summary>
	/// 注册UI面板
	/// </summary>
	void RegisterUiPanels()
	{
		AbstractPanel.RegisterPanel<BattlePanel>(PanelName.battlePanel);
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
			SkillTriggerFactory<BulletTrigger>.Instance);
		SkillTriggerMgr.RegisterTriggerFactory(SkillTriggerName.trigger2D,
			SkillTriggerFactory<Trigger2DTrigger>.Instance);

		//读取文件，获取所有技能
		SkillTriggerMgr.LoadSkillsFromFile(FilePath.SkillFilePath);
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
