using System.Collections;
using System.Collections.Generic;
using Game.Script;
using Game.View;
using UnityEngine;

public class GameInitializer : MonoBehaviour {

	// Use this for initialization
	void OnEnable ()
	{
		Game.Global.CGameObjects.Refresh();
		this.gameObject.AddComponent<MainLoop>();
		this.gameObject.AddComponent<AudioMgr>();
		UIManager.Instance.PushPanel(new WelcomePanel("选一个", "TestScene", "Jb", "游戏开始"));
	}

	void OnDisable()
	{
		UIManager.Instance.PopPanel();
	}
	
}
