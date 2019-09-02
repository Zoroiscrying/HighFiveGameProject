using System;
using UnityEngine;
using zoroiscrying;

namespace Game.Scripts
{
	
[RequireComponent(typeof(CharacterController2D))]
public class PlayerMove : MonoBehaviour
{
	#region Consts
	public const int jumpPoint = 2;
	#endregion
	// movement config

	#region public variables

	public float runSpeed = 8f;
    public float timeToJumpApex = .4f;//跳跃到最高高度用多久
    public float accelerationTimeAirborne = .2f;
    public float accelerationTimeGrounded = .1f;
    public float maxJumpHeight = 1f;
	public float minJumpHeight = .25f;
	public float wallSlidingSpeed = 3f;
	public float wallStickTime = .25f;
	public float dashDistance = 8f;
	public float dashTime = .5f;

	public float horizontalSpeedMultiplier = 1f;
	public float verticalSpeedMultiplier = 1f;
	
	public Vector2 wallJumpClimb;
	public Vector2 wallLeap;

	public float playerVelocityX
	{
		get { return _velocity.x;}
		set { _velocity.x = value; }
	}
	public float playerVelocityY
	{
		get { return _velocity.y;}
		set { _velocity.y = value; }
	}
	
	//public Vector2 wallJumpOff;//
	
	public int canJump = jumpPoint;	
	
	#endregion

	#region private variables
	private float timeToUnstickWall = .25f;
	private CharacterController2D _controller;
   	private Animator _animator;
    private RaycastHit2D _lastControllerColliderHit;
    private Vector3 _velocity;
	private float normalizedHorizontalSpeed = 0;
    private float gravity = -25f;
    private float maxJumpVelocity;
	private float minJumpVelocity;
    private float movementDamping;
	private float dashVelocity;
	private Vector2 directionalInput;
	[SerializeField] private bool wallSliding;
	[SerializeField] private int wallDirX;
	[SerializeField] private bool runningOnWall;
	
	
	
	#endregion

	#region Monobehavior

		void Awake()
    	{
    		_animator = GetComponent<Animator>();
    		_controller = GetComponent<CharacterController2D>();
    
    		// listen to some events for illustration purposes
    		_controller.onControllerCollidedEvent += onControllerCollider;
    		_controller.onTriggerEnterEvent += onTriggerEnterEvent;
    		_controller.onTriggerExitEvent += onTriggerExitEvent;
    		
    		CalculateGravityNVelocity();
    	}

		// the Update loop contains a very simple example of moving the character around and controlling the animation
    	void Update()
    	{	
		    //All moving functions
		    if ((_controller.collisionState.below || _controller.collisionState.above) &&
		        !_controller.collisionState.fallingThroughPlatform)
		    {
			    _velocity.y = 0;
		    }		
		    CalculateVelocity();
		    HandleWallSliding();
		    PlayerAnimStateControl();

		    if (Input.GetKeyDown(KeyCode.Space))
		    {
			    OnJumpInputDown();
		    }
			
		    if (_controller.isGrounded)
		    {
			    canJump = jumpPoint;
		    }
		    _controller.Move(_velocity * Time.deltaTime,directionalInput);
		    // grab our current _velocity to use as a base for all calculations
		    _velocity = _controller.velocity;
		
    
		    Attack();
    	}
	#endregion

	#region Event Listeners

	void onControllerCollider( RaycastHit2D hit )
	{
		// bail out on plain old ground hits cause they arent very interesting
		if( hit.normal.y == 1f )
			return;

		// logs any collider hits if uncommented. it gets noisy so it is commented out for the demo
		//Debug.Log( "flags: " + _controller.collisionState + ", hit.normal: " + hit.normal );
	}


	void onTriggerEnterEvent( Collider2D col )
	{
		Debug.Log( "onTriggerEnterEvent: " + col.gameObject.name );
	}


	void onTriggerExitEvent( Collider2D col )
	{
		Debug.Log( "onTriggerExitEvent: " + col.gameObject.name );
	}

	#endregion

	public void SetDirectionalInput(Vector2 input)
	{
		directionalInput = input;
	}

	public void OnJumpInputDown()
	{
		#region doubleJump

		//Jump point initial set to 1
        // we can only jump whilst grounded
        if( canJump > 1 && !wallSliding && !Input.GetKey(KeyCode.DownArrow))//跳跃
        {
        	_velocity.y = maxJumpVelocity;
        	_animator.Play( Animator.StringToHash( "Jump" ) );
        	canJump--;
        }
        //case over

		#endregion

		#region wallJump

		//WallSlidingCase,Jump
        if (wallSliding)
        {
        	if (wallDirX == normalizedHorizontalSpeed)
        	{
        		_velocity.x = -wallDirX * wallJumpClimb.x;
				_animator.Play( Animator.StringToHash( "Jump" ) );
        		_velocity.y = wallJumpClimb.y;
        	}
        	else if (wallDirX == -normalizedHorizontalSpeed)
        	{
        		_velocity.x = -wallDirX * wallLeap.x;
				_animator.Play( Animator.StringToHash( "Jump" ) );
        		_velocity.y = wallLeap.y;
        	}
        }

		#endregion

		#region OneWayPlatform

		if( _controller.isGrounded && Input.GetKey( KeyCode.DownArrow ))//向下
		{
			_animator.Play(Animator.StringToHash("Jump"));
			//Debug.Log("Move Down!");
			_velocity.y = -2f;
			Invoke("ResetFallingThroughPlatform",1f);
			_controller.ignoreOneWayPlatformsThisFrame = true;
			_controller.collisionState.fallingThroughPlatform = true;

		}

		#endregion

		#region DownWardDash

		if (!_controller.isGrounded && Input.GetKey(KeyCode.DownArrow) )//向下
		{
			DownwardDash();
			_animator.Play("DownwardDashing");
			_animator.SetBool("downwarddashing",true);
		}

		#endregion
		
	}

	public void OnJumpInputUp()
	{
		#region JumpHeightControl

				if (canJump >= 1 )
        		{
        			if (_velocity.y > minJumpVelocity)
        			{
        				_velocity.y = minJumpVelocity;
        			}
        		}

		#endregion
		
	}

	private void PlayerAnimStateControl()
	{
		if( directionalInput.x > 0  )//向右
		{
			_animator.SetBool("dashing",false);
			normalizedHorizontalSpeed = 1;
			if( transform.localScale.x < 0f )
				transform.localScale = new Vector3( -transform.localScale.x, transform.localScale.y, transform.localScale.z );
			if( _controller.isGrounded )
				_animator.Play( Animator.StringToHash( "Move" ));
		}
		else if( directionalInput.x < 0 )//向左
		{
			_animator.SetBool("dashing",false);
			normalizedHorizontalSpeed = -1;
			if( transform.localScale.x > 0f )
				transform.localScale = new Vector3( -transform.localScale.x, transform.localScale.y, transform.localScale.z );
			if( _controller.isGrounded )
				_animator.Play( Animator.StringToHash( "Move" ) );
		}
		else
		{
			normalizedHorizontalSpeed = 0;

			if (_controller.isGrounded && !Input.GetKey(KeyCode.J) && !Input.GetKey(KeyCode.K) &&
			    !Input.GetKey(KeyCode.L)&&!_animator.GetBool("dashing"))
			{
				_animator.Play( Animator.StringToHash( "Idle" ) );
				_animator.SetBool("downwarddashing",false);
			}
		}
	}


	private void HandleWallSliding()
	{
		wallSliding = false;
		wallDirX = (_controller.collisionState.left) ? -1 : 1;

		if ((_controller.collisionState.left || _controller.collisionState.right) &&
		    !_controller.collisionState.below && _velocity.y <= 0)
		{
			wallSliding = true;
			if (_velocity.y < -wallSlidingSpeed) //开始贴墙滑行
			{
				_velocity.y = -wallSlidingSpeed;
				canJump = jumpPoint - 1;
			}

			if (timeToUnstickWall > 0)
			{
				//Debug.Log("Reset velocityX to 0");
				movementDamping = 0;
				_controller.velocity.x = 0;
				if (normalizedHorizontalSpeed != wallDirX && normalizedHorizontalSpeed != 0)
				{
					timeToUnstickWall -= Time.deltaTime;
				}
				else
				{
					timeToUnstickWall = wallStickTime;
				}
			}
			else
			{
				timeToUnstickWall = wallStickTime;
			}
		}			
	}

	private void DownwardDash()
	{
		Console.WriteLine("Down Dash!");
	}

	public void Dash()
	{	//按键触发 - 动画 - 位移（无法改变） - 结束
		//_animator.Play("");
		_animator.Play("Dash");
		_animator.SetBool("dashing",true);
		
	}

	private void AutoClimbWall()
	{
		//射线检测 - 动画 - 位移 - 结束
		
		//_animator.Play("");
		
	}
	private void CalculateGravityNVelocity()
	{
		gravity = - (2 * maxJumpHeight) / Mathf.Pow(timeToJumpApex, 2);
		//print(gravity);
		maxJumpVelocity = Mathf.Abs(gravity) * timeToJumpApex;
		minJumpVelocity = Mathf.Sqrt(2 * Mathf.Abs(gravity) * minJumpHeight);
		dashVelocity = dashDistance / dashTime;
	}

	void CalculateVelocity()
	{
		float targetVelocityX = directionalInput.x * runSpeed * horizontalSpeedMultiplier;
		// apply horizontal speed smoothing it. dont really do this with Lerp. Use SmoothDamp or something that provides more control
		//var smoothedMovementFactor = _controller.isGrounded ? movementDamping : inAirDamping; // how fast do we change direction?
		_velocity.x = Mathf.SmoothDamp( _velocity.x, targetVelocityX, ref movementDamping,(_controller.collisionState.below)?accelerationTimeGrounded:accelerationTimeAirborne);
		// apply gravity before moving
		_velocity.y += gravity * Time.deltaTime;
		// if holding down bump up our movement amount and turn off one way platform detection for a frame.
		// this lets us jump down through one way platforms
	}


	void ResetFallingThroughPlatform()
	{
		_controller.collisionState.fallingThroughPlatform = false;
		//Debug.Log("Reset Completed");
	}

	void Attack()
	{
		if (Input.GetKeyDown(KeyCode.J))
		{
			_animator.Play(Animator.StringToHash("Attack1"));
			Debug.Log("Attack!");
		}
		if (Input.GetKeyDown(KeyCode.K))
		{
			_animator.Play(Animator.StringToHash("Attack2"));
			Debug.Log("Attack!");
		}
		if (Input.GetKeyDown(KeyCode.L))
		{
			_animator.Play(Animator.StringToHash("Attack3"));
			Debug.Log("Attack!");
		}
	}
}

}
