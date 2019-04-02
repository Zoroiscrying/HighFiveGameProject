using System.Collections;
using System.Collections.Generic;
using Game;
using Game.Global;
using UnityEngine;

public class BaseSceneMgr : MonoBehaviour {

	void Awake()
	{
		if (GlobalFlag.isPlaying == false)
		{
			Register();
			GlobalFlag.isPlaying = true;
		}

		Initializer();
	}

	protected virtual void Register()
	{
		
	}

	protected virtual void Initializer()
	{
		
	}
}
