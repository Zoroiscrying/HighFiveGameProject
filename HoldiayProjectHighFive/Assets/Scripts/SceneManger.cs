using System.Collections;
using System.Collections.Generic;
using Game;
using Game.Const;
using Game.Control;
using Game.Global;
using Game.Modal;
using Game.Script;
using Game.Serialization;
using Game.View;
using UnityEngine;
using Mono.Data.Sqlite;
using UnityEditor;
using UnityEngine.SceneManagement;

public class SceneManger : MonoSingleton<SceneManger>
{


	List<AbstractPerson> list=new List<AbstractPerson>();
	//private Player player;
	public string UiPanelName;

	private Player player;

	void Start()
	{
		this.player=AbstractPerson.GetInstance<Player>(CGameObjects.Player);
	}
	// Use this for initialization
	void Update()
	{
		if (Input.GetKeyDown(KeyCode.UpArrow))
		{
			this.player.AddSpirit(SpiritName.C_First);
			Debug.Log("添加灵器" + SpiritName.C_First);
			
		}

		if (Input.GetKeyDown(KeyCode.DownArrow))
		{
			this.player.RemoveSpirit(SpiritName.C_First);
			Debug.Log("移出灵器" + SpiritName.C_First);
		}
     			
		
		if (Input.GetKeyDown(KeyCode.K))
		{
			foreach (var t in list)
				if(t.obj!=null)
					new Bullet(10, t.Dir, t.Pos+new Vector3(0,0.3f,0), t);
		}


		if (Input.GetKeyDown(KeyCode.S))
		{
			XmlManager.SaveData(this.player,GameData.PlayerDataFilePath);
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
	void Awake()
	{
		if (Game.Global.Flag.isPlaying == false)
		{
			InitSkillSystem();
			InitUiPanels();
			InitSpiritMent();
		}
		InitSceneScripts();
		InitGlobalVar();
		InitBehavic();
	}

	/// <summary>
	/// 初始化UI面板
	/// </summary>
	void InitUiPanels()
	{
		BasePanel.InitPanel<BattlePanel>(PanelName.battlePanel);
	}


	void InitSpiritMent()
	{
		AbstractSpiritItem.RigistSpiritMent<ShitSpirit>(SpiritName.C_First);
		Debug.Log("已经注册" + SpiritName.C_First);
	}

	/// <summary>
	/// 初始化全局脚本
	/// </summary>
	void InitSceneScripts()
	{
		
		this.gameObject.AddComponent<MainLoop>();
		this.gameObject.AddComponent<AudioMgr>();
	}

	/// <summary>
	/// 初始化技能系统
	/// </summary>
	void InitSkillSystem()
	{
		//所有触发器种类的注册
		SkillTriggerMgr.Instance.RegisterTriggerFactory("AnimationTrigger",
			SkillTriggerFactory<AnimationTrigger>.Instance);
		SkillTriggerMgr.Instance.RegisterTriggerFactory("InstantDamageTrigger",
			SkillTriggerFactory<InstantRayDamageTrigger>.Instance);
		SkillTriggerMgr.Instance.RegisterTriggerFactory("AudioTrigger",
			SkillTriggerFactory<AudioTrigger>.Instance);
		SkillTriggerMgr.Instance.RegisterTriggerFactory("DashTrigger",
			SkillTriggerFactory<DashTrigger>.Instance);
		SkillTriggerMgr.Instance.RegisterTriggerFactory("BulletTrigger",
			SkillTriggerFactory<BulletTrigger>.Instance);
		SkillTriggerMgr.Instance.RegisterTriggerFactory("Trigger2DTrigger",
			SkillTriggerFactory<Trigger2DTrigger>.Instance);
//            SkillTriggerMgr.Instance.RegisterTriggerFactory("LockFrameTrigger",
//                SkillTriggerFactory<LockFrameTrigger>.Instance);

		//读取文件，获取所有技能
		SkillSystem.Instance.LoadSkillsFromFile(FilePath.SkillFilePath);
	}

	/// <summary>
	/// 初始化全局变量
	/// </summary>
	void InitGlobalVar()
	{
		Game.Global.CGameObjects.Refresh();
        CreateTestPeople();
		BaseSceneInfo.InitPlayer();
	}
	
	/// <summary>
	/// 初始化行为树
	/// </summary>
	/// <returns></returns>
	bool InitBehavic()
    {
     	behaviac.Workspace.Instance.FilePath = Application.dataPath + "/Scripts/behaviac/exported/behaviac_generated/types";
     	behaviac.Workspace.Instance.FileFormat = behaviac.Workspace.EFileFormat.EFF_xml;
     	return true;
    }
	
	#endregion
	

}
