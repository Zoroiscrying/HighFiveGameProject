using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Game.StateMachine;

public class MainCharacter : Actor {

	
	#region Consts

	public const int _jumpPoint = 2;

	#endregion

	#region Public Variables

	public float _wallStickTime = 2f;
	public float _wallSlideVelocity = 2f;
	public float _dashTime = .5f;
	public float _dashDistance = 8f;
	public float _minJumpHeight = .25f;
	public float _wallJumpTime = .25f;
	public Vector2 _wallJumpClimb;
	public Vector2 _wallJumpNormal;
	
	public float _playerVelocityX
	{
		get { return _velocity.x;}
		set { _velocity.x = value; }
	}
	public float _playerVelocityY
	{
		get { return _velocity.y;}
		set { _velocity.y = value; }
	}

	public int _canJump = _jumpPoint;

	public bool _inControl = true;
	#endregion

	#region Private Variables

	private StateMachine<PlayerStates> _stateMachine;
	
	//timer
	
	private float _minJumpVelocity;
	private float _dashVelocity;
	private bool _wallSliding;
	private bool _runningOnWall;
	private bool _standingOnMovingPlatform = false;
	
	private int _wallDirX;

	private float _wallJumpDisableInputTimer;
	
	#endregion

	#region Protected Variables

	

	#endregion

	#region Struct, Class, Enums

		public enum PlayerStates
		{
			Idle,
			Run,
			Jump,
			DoubleJump,
			WallSliding,
			Dashing,
			FallingThroughPlatform,
			ClimbingWall,
			WallJump
		}

	#endregion

	#region Monobehaviors

	public override void Awake()
	{
		base.Awake();
		_stateMachine = StateMachine<PlayerStates>.Initialize(this,PlayerStates.Idle);
		
	}

	
	// Update is called once per frame
	public override void Update ()
	{
		base.Update();
		
		//Variables
		
		//standingOnPlatform
		
		
		WallSlide();
		
		ResetJumpPoint();		
		
		GetPlayerInput();
		
		BasicStateCheck();

	}

	private void LateUpdate()
	{

	}

	#endregion

	#region EventListeners

	

	#endregion

	#region Public Functions

	public void SetDirectionalInput(Vector2 input)
	{
		_directionalInput = input;
	}

	public void OnJumpKeyDown()
	{
		
	}

	public void OnJumpKeyUp()
	{
		
	}
	

	#endregion

	#region Private Functions


	private void ResetJumpPoint()
	{
		if (_controller.isGrounded )
		{
			_canJump = _jumpPoint;
		}
	}

	private void BasicStateCheck()
	{
		//Idle
		if (_controller.isGrounded && _directionalInput.x == 0 && (_stateMachine.State == PlayerStates.Run ||
		   _stateMachine.State == PlayerStates.Jump || _stateMachine.State == PlayerStates.DoubleJump||
		   _stateMachine.State == PlayerStates.WallSliding || _stateMachine.State == PlayerStates.WallJump))//注意，是玩家没有输入左右键判断不是通过速度判断
		{
			_stateMachine.ChangeState(PlayerStates.Idle);
		}
		
		//Run
		if (_controller.isGrounded && _directionalInput.x != 0 && 
		    (_stateMachine.State == PlayerStates.Idle ||_stateMachine.State == PlayerStates.Jump ))
		{
			_stateMachine.ChangeState(PlayerStates.Run);
		}
		
		//Jump
		if (!_controller.isGrounded &&
		    (_stateMachine.State == PlayerStates.Idle || _stateMachine.State == PlayerStates.Run))
		{
			_stateMachine.ChangeState(PlayerStates.Jump);
		}
		
	}

	private void Jump()
	{
		if (_canJump > 1)
		{
			//Debug.Log("Double Jump.");
			_stateMachine.ChangeState(PlayerStates.Jump);
			_velocity.y = _maxJumpVelocity;
			_canJump--;
		}
		else if (_canJump == 1)
		{
			//Debug.Log("Normal Jump.");
			_stateMachine.ChangeState(PlayerStates.Jump);
			_velocity.y = _maxJumpVelocity;
			_canJump--;
		}
	
	}

	private void WallJump()
	{
		if (_controller.IfNearWall(_velocity)!=0 && (_stateMachine.State == PlayerStates.Jump ||
		_stateMachine.State == PlayerStates.DoubleJump || _stateMachine.State == PlayerStates.WallSliding) )
		{
			_wallDirX = _controller.IfNearWall(_velocity);
			_stateMachine.ChangeState(PlayerStates.WallJump);
			if ((_controller.collisionState.right && _wallDirX == 1)||(_controller.collisionState.left&&_wallDirX == -1))
			{//参考蔚蓝的蹬墙跳设计，如果玩家按键朝向墙，则蹬墙跳力气更大一些
				_velocity.y = _wallJumpClimb.y;
				_velocity.x = _wallJumpClimb.x * -_wallDirX;
				return;
			}
			//如果玩家按键无朝向，则蹬墙跳力气小一些。
			_velocity.y = _wallJumpNormal.y;
			_velocity.x = _wallJumpNormal.x * -_wallDirX;
		}
	}

	private void WallSlide()
	{
		_wallDirX = (_controller.collisionState.left) ? -1 : 1;

		
		if ((_stateMachine.State == PlayerStates.Jump || _stateMachine.State == PlayerStates.DoubleJump)&&
		    ((_controller.collisionState.right && _directionalInput.x > 0) ||
		     ( _controller.collisionState.left&& _directionalInput.x < 0))&& _velocity.y <=0)
		{
			_stateMachine.ChangeState(PlayerStates.WallSliding);
		}
		
		
	}

	private void Dash()
	{
		_stateMachine.ChangeState(PlayerStates.Dashing);
		//无敌帧
	}

	private void GetPlayerInput()
	{
		if (_inControl)
		{

			_directionalInput = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));

			if (Input.GetKeyDown(KeyCode.Space) && _stateMachine.State != PlayerStates.DoubleJump)
			{
				WallJump();
				Jump();
			}

			if (Input.GetKeyUp(KeyCode.Space))
			{
				this.OnJumpKeyUp();
			}

			if (Input.GetKeyDown(KeyCode.LeftShift))
			{
				Dash();
			}
		}
		else
		{
			_directionalInput = new Vector2(0,0);
		}
	}


	#endregion

	#region StateCallBacks

		#region IdleState

		private float _positionYLastFrame;
		private void Idle_Enter()
		{
			_positionYLastFrame = this.transform.position.y;
			_animator.Play(Animator.StringToHash("Idle"));
		}
	
		private void Idle_Update()
		{
			if (_directionalInput.x != 0)
			{
				_stateMachine.ChangeState(PlayerStates.Run);
			}

		}
	
		private void Idle_Exit()
		{
			//Debug.Log("Idle State Exit.");
		}

		#endregion

		#region JumpState

		private void Jump_Enter()
		{
			_animator.Play(Animator.StringToHash("Jump"));
			_controller.collisionState.below = false;
		}
	
		private void Jump_Update()
		{
			if (_controller.isGrounded)
			{
				ResetJumpPoint();
			}

			if (Input.GetKeyDown(KeyCode.Space))
			{
				//_stateMachine.ChangeState(PlayerStates.DoubleJump);
			}
		}
	
		private void Jump_Exit()
		{
			//Debug.Log("Jump Exit");
		}

		#endregion
	
		#region RunState
	
		private void Run_Enter()
		{
			_animator.Play(Animator.StringToHash("Move"));
		}
	
		private void Run_Update()
		{
			if (_directionalInput.x == 0)
			{
				_stateMachine.ChangeState(PlayerStates.Idle);
			}
		}
	
		private void Run_Exit()
		{
			
		}
	
		#endregion

		#region DashingState

		private float _dashTimer;
		private int _dashDirX;
		private void Dashing_Enter()
		{	
			//Debug.Log("Dash Begin");
			_animator.Play(Animator.StringToHash("Dash"));
			_dashTimer = _dashTime;
			_inControl = false;
			_dashVelocity = _dashDistance / _dashTime;
			_dashDirX = _faceDir;
		}
	
		private void Dashing_Update()
		{
			if (_dashDirX == 1 && _controller.collisionState.right)
			{
				_dashVelocity = 0;
			}
			else if (_dashDirX == -1 && _controller.collisionState.left)
			{
				_dashVelocity = 0;
			}
			
			if (_dashTimer > 0)
			{
				_dashTimer -= Time.deltaTime;
				_velocity.x = _dashVelocity * _dashDirX;
			}
			else
			{
				_stateMachine.ChangeState(PlayerStates.Idle);
			}
			
			
		}
	
		private void Dashing_Exit()
		{
			_inControl = true;
		}
	
		#endregion

		#region DoubleJumpState
	
		private void DoubleJump_Enter()
		{
			//Debug.Log("DoubleJump Enter");
		}
	
		private void DoubleJump_Update()
		{
			
		}
	
		private void DoubleJump_Exit()
		{
			
		}
	
		#endregion

		#region WallSlidingState
		private float WallSlidingTimer= 0f;
		private void WallSliding_Enter()
		{
			//Debug.Log("WallSliding.");
			
			
		}
		
		private void WallSliding_Update()
		{
			WallSlidingTimer += Time.deltaTime;

			if (_velocity.y < -_wallSlideVelocity)
			{
				_velocity.y = -_wallSlideVelocity;
			}

			if (_directionalInput.x * _wallDirX <= 0)
			{
				_stateMachine.ChangeState(PlayerStates.Jump);
			}
			
			if (_controller.isGrounded)
			{
				ResetJumpPoint();
			}
			
		}
		
		private void WallSliding_Exit()
		{
			WallSlidingTimer = 0f;
		}
	
		#endregion

		#region WallJumpState

		private float WallJumpTimer;
	
		private void WallJump_Enter()
		{
			//Debug.Log("Wall Jump");
			_animator.Play(Animator.StringToHash("Jump"));
			WallJumpTimer = _wallJumpTime;
			_inControl = false;
		}
		
		private void WallJump_Update()
		{
			if (WallJumpTimer >0)
			{
				WallJumpTimer -= Time.deltaTime;
			}
			else
			{
				_inControl = true;
				_stateMachine.ChangeState(PlayerStates.Jump);
			}
			
			if (_controller.isGrounded)
			{
				ResetJumpPoint();
			}
			
		}
	
		private void WallJump_Exit()
		{
			//Debug.Log("Wall Jump Exit");
			_inControl = true;
		}
	
		#endregion

	#endregion

}
