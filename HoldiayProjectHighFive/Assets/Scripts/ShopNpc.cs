using System.Collections;
using System.Collections.Generic;
using Game.Const;
using Game.Script;
using Game.View.PanelSystem;
using UnityEngine;

public class ShopNpc : CloseEnough
{
	private bool isShowing;

	protected override void Start()
	{
		base.Start();
		isShowing = false;
	}
	void Update()
	{
		if (isClose)
		{
			//Debug.Log("Close");
			if (Input.GetKeyDown(KeyCode.E))
			{
				Debug.Log("KeyDown");
				if (isShowing)
				{
					UIManager.Instance.PopPanel();
					isShowing = false;
				}
				else
				{
					UIManager.Instance.PushPanel(PanelName.shopPanel);
					isShowing = true;
				}
			}
		}
	}

	protected override void OnTriggerExit2D(Collider2D col)
	{
		base.OnTriggerExit2D(col);
		if(isShowing)
			UIManager.Instance.PopPanel();
	}
}
