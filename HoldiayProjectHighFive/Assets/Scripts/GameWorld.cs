using System.Collections;
using System.Collections.Generic;
using Game;
using Game.Script;
using UnityEngine;

public class GameWorld : MonoSingleton<GameWorld> {
	void Awake()
	{
		this.gameObject.AddComponent<MainLoop>();
		this.gameObject.AddComponent<AudioMgr>();
		this.gameObject.AddComponent<DemoTest>();
		InitBehavic();
	}	
	private bool InitBehavic()
	{
		Debug.Log("InitBehavic");
		behaviac.Workspace.Instance.FilePath = Application.dataPath + "/Scripts/behaviac/exported/behaviac_generated/types";
		behaviac.Workspace.Instance.FileFormat = behaviac.Workspace.EFileFormat.EFF_xml;
		return true;
	}

}
