using System.Collections;
using System.Collections.Generic;
using Game.Common;
using UnityEngine;

public class Trigger2DMessage : MonoBehaviour
{
	public LayerMask testLayers;
	public string message;
	void OnTriggerEnter2D(Collider2D col)
	{
		if (col.IsTouchingLayers(this.testLayers))
		{
			CEventCenter.BroadMessage(this.message);
		}
	}
}
