using System;
using System.Collections;
using System.Collections.Generic;
using Game;
using Game.Control;
using Game.Control.PersonSystem;
using Game.Model.SceneSystem;
using Game.View.PanelSystem;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class SceneTrigger : MonoBehaviour
{

	public string newSceneName;
	public string newUIName;
	public Vector3 newPosition;

	void OnTriggerExit2D(Collider2D col)
	{
		if (null == AbstractPerson.GetInstance<Player>(col.gameObject))
			return;
		Game.Const.DefaultData.PlayerPos = this.newPosition;
		
        SceneMgr.LoadScene(newSceneName, newUIName);
	}
}
