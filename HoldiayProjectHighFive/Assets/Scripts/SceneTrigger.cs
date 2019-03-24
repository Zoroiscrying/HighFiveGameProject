using System;
using System.Collections;
using System.Collections.Generic;
using Game;
using Game.Control;
using Game.Modal;
using UnityEngine;
using UnityEngine.EventSystems;


public class SceneTrigger : MonoBehaviour
{

	public string newSceneName;
	public Vector3 newPosition;

	void OnTriggerExit2D(Collider2D col)
	{
		if (null == AbstractPerson.GetInstance<Player>(col.gameObject))
			return;
		Game.Const.GameData.PlayerPos = this.newPosition;
		SceneMgr.Instance.LoadScene(this.newSceneName);
	}
	
	
}
