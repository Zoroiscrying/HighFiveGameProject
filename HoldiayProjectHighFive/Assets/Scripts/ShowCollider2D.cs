using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class ShowCollider2D : MonoBehaviour
{

	private BoxCollider2D box;
	// Use this for initialization
	void Start () {
		
	}

	private void OnDrawGizmos()
	{
		if (this.box == null)
			this.box = GetComponent<BoxCollider2D>();
		var scanle = this.transform.localScale;
		var offect = scanle * box.offset;
		var size = scanle * box.size;
		var center = new Vector2(transform.position.x + offect.x, transform.position.y + offect.y);
		var lt = center + new Vector2(-size.x / 2, size.y / 2);
		var lb = center + new Vector2(-size.x / 2, -size.y / 2);
		var rt = center + new Vector2(size.x / 2, size.y / 2);
		var rb = center + new Vector2(size.x / 2, -size.y / 2);
		Debug.DrawLine(lt, rt, Color.cyan);
		Debug.DrawLine(lt, lb, Color.cyan);
		Debug.DrawLine(rt, rb, Color.cyan);
		Debug.DrawLine(lb,rb,Color.cyan);
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
