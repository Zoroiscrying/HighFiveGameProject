using System.Collections;
using System.Collections.Generic;
using Game;
using UnityEngine;
using zoroiscrying;

[RequireComponent(typeof(CharacterController2D),typeof(Animator))]
public class Actor : MonoBehaviour {
	//Actor，场景中的所有角色，受重力影响，有基本的碰撞和相应事件
	
	#region Consts
	
	

	#endregion

	#region Public Variables

	public float _runSpeed = 8f;
	public float _timeToJumpApex = .4f;
	public float _accelerationTimeAirborne = .2f;
	public float _accelerationTimeGrounded = .1f;
	public float _maxJumpHeight = 1f;

	public float _horizontalSpeedMultiplier = 1f;
	public float _verticalSpeedMultiplier = 1f;
	
	public bool _affectedByGravity = true;

	public GameAnimator _animator;
	public int _normalizedDirX
	{
		get
		{
			if (_directionalInput.x > 0)
			{
				return 1;
			}
			if (_directionalInput.x < 0)
			{
				return -1;
			}
			return 0;
			
		}
	}

	public int _faceDir = 1;
	
	#endregion

	#region Private Variables


	private float _gravity = -25f;

	private float _movementDamping;
	
	
	#endregion

	#region Protected Variables
	
	protected float _maxJumpVelocity;	
	protected Vector3 _velocity;
	protected CharacterController2D _controller;
	protected Vector2 _directionalInput;//暂时无法确定要不要加这个接受输入的变量
	
	#endregion

	#region Struct,Class,Enums..

	

	#endregion

	#region Monobehaviors

	public virtual void Awake()
	{
		_controller = GetComponent<CharacterController2D>();
		_animator = GameAnimator.GetInstance(GetComponent<Animator>());

		_controller.onControllerCollidedEvent += onControllerCollider;
		_controller.onTriggerExitEvent += onTriggerEnterEvent;
		_controller.onTriggerExitEvent += onTriggerExitEvent;
		
		CalculateGravityNVelocity();
	}

	public virtual void Update () 
	{
		CalculateVelocity();
		
		CalculateMovementBasedOnVelocity();//这个是每帧根据Velocity对角色进行移动，但是被继承之后还是把移动放到所有计算最后比较好，所以注释掉
		
		CheckCollisions();
		
		AnimFaceDirControl();
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
//		Debug.Log( "onTriggerEnterEvent: " + col.gameObject.name );
	}


	void onTriggerExitEvent( Collider2D col )
	{
//		Debug.Log( "onTriggerExitEvent: " + col.gameObject.name );
	}


	#endregion

	#region Public Functions

	public void MoveTo()
	{
		
	}

	#endregion

	#region Private Functions
	
	private void CalculateGravityNVelocity()
	{
		if (_affectedByGravity)
		{
			_gravity = -(2 * _maxJumpHeight) / Mathf.Pow(_timeToJumpApex, 2);
		}
		else
		{
			_gravity = 0;
		}
		//print(gravity);
		_maxJumpVelocity = Mathf.Abs(_gravity) * _timeToJumpApex;
	}

	private void CheckCollisions()
	{
		if ((_controller.collisionState.below || _controller.collisionState.above) &&
		    !_controller.collisionState.fallingThroughPlatform)
		{
			_velocity.y = 0;
		}
	}

	private void CalculateMovementBasedOnVelocity()
	{
		_controller.Move(_velocity * Time.deltaTime,_directionalInput);
		// grab our current _velocity to use as a base for all calculations
		_velocity = _controller.velocity;
	}
	
	private void CalculateVelocity()
	{
		float targetVelocityX = _directionalInput.x * _runSpeed * _horizontalSpeedMultiplier;
		// apply horizontal speed smoothing it. dont really do this with Lerp. Use SmoothDamp or something that provides more control
		//var smoothedMovementFactor = _controller.isGrounded ? movementDamping : inAirDamping; // how fast do we change direction?
		_velocity.x = Mathf.SmoothDamp( _velocity.x, targetVelocityX, ref _movementDamping,
			(_controller.collisionState.below)?_accelerationTimeGrounded:_accelerationTimeAirborne);
		// apply gravity before moving
		_velocity.y += _gravity * Time.deltaTime;
		// if holding down bump up our movement amount and turn off one way platform detection for a frame.
		// this lets us jump down through one way platforms
	}

	private void AnimFaceDirControl()
	{
		if( _normalizedDirX == 1 )//向右
		{
			//Debug.Log("Turn right!");
			_faceDir = 1;
			if( transform.localScale.x < 0f )
				transform.localScale = new Vector3( -transform.localScale.x, transform.localScale.y, transform.localScale.z );
		}
		else if(_normalizedDirX == -1 )//向左
		{
			_faceDir = -1;
			if( transform.localScale.x > 0f )
				transform.localScale = new Vector3( -transform.localScale.x, transform.localScale.y, transform.localScale.z );
		}
	}
	#endregion
	

}
