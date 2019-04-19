using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Game.Control;
using UnityEngine.Assertions;
using Game.Control.Person;

public class BackGroundController : MonoBehaviour
{
	private Player player;
	public float X_Speed;
	public float Y_Speed;
	private Vector3 beforePos;

	void Start()
	{
		this.player = Game.Global.GlobalVar.G_Player;
		this.beforePos = this.player.Pos;
	}
	// Update is called once per frame
	void Update ()
	{
		var nowPos = this.player.Pos;
		if (nowPos == Game.Const.Signal.defaultPos)
			return;
		var temp = (nowPos - this.beforePos);
		temp = new Vector3(temp.x * this.X_Speed, temp.y * this.Y_Speed, 0);
		this.transform.Translate(temp*0.01f,Space.World);
		this.beforePos = nowPos;
	}
}
