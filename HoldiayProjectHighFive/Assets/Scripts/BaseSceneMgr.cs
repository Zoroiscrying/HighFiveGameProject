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
			InitOnlyOnce();
			GlobalFlag.isPlaying = true;
		}

		InitEachTime();
	}

	protected virtual void InitOnlyOnce()
	{
		
	}

	protected virtual void InitEachTime()
	{
		
	}
}
