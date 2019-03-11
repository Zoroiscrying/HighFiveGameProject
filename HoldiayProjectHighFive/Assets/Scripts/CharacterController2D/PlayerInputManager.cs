using UnityEngine;
using System.Collections;

[RequireComponent (typeof (PlayerMove))]
public class PlayerInputManager : MonoBehaviour {

	
	PlayerMove player;

	void Start () {
		player = GetComponent<PlayerMove> ();
	}

	void Update () {
		Vector2 directionalInput = new Vector2 (Input.GetAxisRaw ("Horizontal"), Input.GetAxisRaw ("Vertical"));
		player.SetDirectionalInput (directionalInput);

		//if (Input.GetKeyDown (KeyCode.Space)) {
		//	player.OnJumpInputDown ();
		//	Debug.Log("Space");
		//}
		if (Input.GetKeyUp (KeyCode.Space)) {
			player.OnJumpInputUp ();
		}

		if (Input.GetKeyDown(KeyCode.LeftShift))
		{
			player.Dash();
		}
	}
}