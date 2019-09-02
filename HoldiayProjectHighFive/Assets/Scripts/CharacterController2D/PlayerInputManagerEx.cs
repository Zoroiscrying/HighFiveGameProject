using UnityEngine;


[RequireComponent(typeof(MainCharacter))]
public class PlayerInputManagerEx : MonoBehaviour
{

	private MainCharacter _player;

	private Vector2 _directionalInput;
	
	// Use this for initialization
	private void Awake()
	{
		_player = GetComponent<MainCharacter>();
	}

	// Update is called once per frame
	void Update () {

		if (_player._inControl)
		{
			_directionalInput = new Vector2(Input.GetAxisRaw("Horizontal"),Input.GetAxisRaw("Vertical"));
			_player.SetDirectionalInput(_directionalInput);
	
			if (Input.GetKeyDown(KeyCode.Space))
			{
				_player.OnJumpKeyDown();
			}
			
			if (Input.GetKeyUp(KeyCode.Space))
			{
				_player.OnJumpKeyUp();
			}
						
		}

		//Debug.Log("Input :" + _directionalInput.x + "," + _directionalInput.y);
		
	}
}
