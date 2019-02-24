using System;
using System.Collections;
using System.Collections.Generic;
using Game.Control;
using UnityEngine;
using UnityEngine.Assertions;


[Serializable]
public struct SkillInfo
{
	public string skillName;
	[Range(0, 1)] public float beginComboTest;
	public float canMoveTime;
	public bool ignoreInput;
	public float faultToleranceTime;
}
[Serializable]
public class PlayerAttackDebug : MonoBehaviour
{
	public float airXMove;
	public List<SkillInfo> skillInfoList=new List<SkillInfo>();
	// Use this for initialization
	void Start ()
	{
//		print(this.gameObject);
//		Debug.Log(Game.Global.CGameObjects.Player.GetInstanceID()+" "+this.gameObject.GetInstanceID());
		Assert.IsTrue(Game.Global.CGameObjects.Player == this.gameObject);
		var ap = AbstractPerson.GetInstance(this.gameObject)as Player;
		ap.airXMove = this.airXMove;
		foreach (var cur in skillInfoList)
		{
			ap.canMoveTime.Add(cur.canMoveTime);
			ap.skillNames.Add(cur.skillName);
			ap.ignoreInput.Add(cur.ignoreInput);
			ap.durTimes.Add(cur.faultToleranceTime);
			ap.beginComboTest.Add(cur.beginComboTest);
		}
	}
}
