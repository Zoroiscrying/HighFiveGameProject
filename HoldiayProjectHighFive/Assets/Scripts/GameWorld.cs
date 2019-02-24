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
	}
}
