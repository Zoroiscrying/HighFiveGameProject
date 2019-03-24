using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test : MonoBehaviour
{

	public float speed = 1;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update ()
	{
		var h = Input.GetAxis("Horizontal");
		var v = Input.GetAxis("Vertical");
		this.transform.Translate(new Vector3(h, v, 0) * speed, Space.World);
	}

	void OnTriggerEnter2D(Collider2D col)
	{
		print("触发");
	}

	void OnCollisionEnter2D(Collision2D c)
	{
		print("碰撞");
	}
}
