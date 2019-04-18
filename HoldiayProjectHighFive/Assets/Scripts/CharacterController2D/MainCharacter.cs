using System;
using System.Collections;
using System.Collections.Generic;
using Game;
using Game.Const;
using Game.Control;
using Game.Global;
using UnityEngine;
using Game.StateMachine;
using Game.Control.Person;
using Game.Common;

public class MainCharacter : Actor 
{

	
	
	#region Consts

	public const int _jumpPoint = 2;

	#endregion

	#region Public Variables

	//上升时的最大速度
	public float _maxUpYSpeed = 999.0f;
	//下降时的最大速度
	public float _maxDownYSpeed = 9.0f;
	
	//原有基础上增加的速度
	public float AccelerationSpeed = 2.0f;

	public float AccelerationTime = 3.0f;
	
	//高跳的高度
	public float HighJumpHeight = 6.0f;
	
	//public float _wallStickTime = 2f;//暂时不用
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
	private float _aclratnTimer = 0.0f;
	
	
	private float _highJumpVelocity;
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
		//	Jump,
			DoubleJump,
			WallSliding,
			Dashing,
			FallingThroughPlatform,
			ClimbingWall,
			WallJump,
			Fall,
			InAir
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

		//灵力归元
		if (Player.isSuper)
		{
			if (Input.GetKeyDown(KeyCode.Z))
			{
				Debug.Log("加速");
				StartAcceleration(AccelerationTime);
			}
		}
		
		
		CheckIsInAir();
		
		WallSlide();
		
		ResetJumpPoint();
		
		GetPlayerInput();
		
		BasicStateCheck();

		CalculateGravityNVelocity();
		
		

	}

	#endregion

	#region EventListeners

	

	#endregion

	#region Public Functions

	public override void CalculateGravityNVelocity()
	{
		base.CalculateGravityNVelocity();
		float minGravity = -(2 * _minJumpHeight) / Mathf.Pow(_timeToJumpApex, 2);
		_minJumpVelocity = Mathf.Abs(minGravity) * _timeToJumpApex;
		float highJumpGravity = -(2 * HighJumpHeight) / Mathf.Pow(_timeToJumpApex, 2);
		_highJumpVelocity = Mathf.Abs(highJumpGravity) * _timeToJumpApex;
	}
	
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

	private void CheckIsInAir()
	{
		//在空中玩家只能施展二段跳
		if (!_controller.isGrounded && _stateMachine.State != PlayerStates.DoubleJump
		    && _stateMachine.State != PlayerStates.WallSliding&& _stateMachine.State != PlayerStates.WallJump)
		{
			_stateMachine.ChangeState(PlayerStates.InAir);
		}
	}
	
	private void ResetJumpPoint()
	{
		if (_controller.isGrounded)
		{
			_canJump = _jumpPoint;
		}
	}

	private void BasicStateCheck()
	{
		//Idle & run
		if (_controller.isGrounded  && (_stateMachine.State == PlayerStates.Run ||
		   //_stateMachine.State == PlayerStates.Jump || 
		                                _stateMachine.State == PlayerStates.DoubleJump||
		   _stateMachine.State == PlayerStates.WallSliding || _stateMachine.State == PlayerStates.WallJump || 
		                                                           _stateMachine.State == PlayerStates.Fall))//注意，是玩家没有输入左右键判断不是通过速度判断
		{
			//idle
			if (_directionalInput.x == 0)
			{
				_stateMachine.ChangeState(PlayerStates.Idle);				
			}
			//run
			if (_directionalInput.x != 0)
			{
				_stateMachine.ChangeState(PlayerStates.Run);
			}
		}
		
		//Jump
		if (!_controller.isGrounded &&
		    (_stateMachine.State == PlayerStates.Idle || _stateMachine.State == PlayerStates.Run))
		{
			_stateMachine.ChangeState(PlayerStates.InAir);
		}
		
	}

	private void StartAcceleration(float time)
	{
		CEventCenter.BroadMessage(Message.M_ExitSuper);
		Game.Script.MainLoop.Instance.ExecuteLater(() => _runSpeed -= AccelerationSpeed, time);
		_runSpeed += AccelerationSpeed;
	}
	
	
	/// <summary>
	/// 玩家按下跳跃键后触发
	/// </summary>
	private void Jump()
	{
		if (Player.isSuper)
		{
			_stateMachine.ChangeState(PlayerStates.InAir);
			_velocity.y = _highJumpVelocity;
			_canJump--;
			CEventCenter.BroadMessage(Message.M_ExitSuper);
		}
		else if (_stateMachine.State == PlayerStates.InAir)
		{
			_stateMachine.ChangeState(PlayerStates.DoubleJump);
			_velocity.y = _maxJumpVelocity;
			_canJump = _jumpPoint - 1;
		}
		else if (_canJump >= 1)
		{
			//Debug.Log("Normal Jump.");
			_stateMachine.ChangeState(PlayerStates.InAir);
			_velocity.y = _maxJumpVelocity;
			_canJump--;
		}
	
	}

	/// <summary>
	/// 蹬墙跳
	/// </summary>
	private void WallJump()
	{
		if (_controller.IfNearWall(_velocity)!=0 && (
			  //  _stateMachine.State == PlayerStates.Jump ||
		_stateMachine.State == PlayerStates.DoubleJump || _stateMachine.State == PlayerStates.WallSliding
		                                             || _stateMachine.State == PlayerStates.InAir) )
		{
			_wallDirX = _controller.IfNearWall(_velocity);
			_stateMachine.ChangeState(PlayerStates.WallJump);
			if ((_controller.collisionState.right && _wallDirX == 1)||(_controller.collisionState.left&&_wallDirX == -1))
			{
				//参考蔚蓝的蹬墙跳设计，如果玩家按键朝向墙，则蹬墙跳力气更大一些
				_velocity.y = _wallJumpClimb.y;
				_velocity.x = _wallJumpClimb.x * -_wallDirX;
				return;
			}
			//如果玩家按键无朝向，则蹬墙跳力气小一些。
			_velocity.y = _wallJumpNormal.y;
			_velocity.x = _wallJumpNormal.x * -_wallDirX;
		}
	}

	/// <summary>
	/// 滑墙判断和速度设定
	/// </summary>
	private void WallSlide()
	{
		if (_controller.collisionState.left)
		{
			_wallDirX = -1;
		}
		else if (_controller.collisionState.right)
		{
			_wallDirX = 1;
		}
		else if (!_controller.collisionState.right && !_controller.collisionState.left )
		{
			_wallDirX = 0;
		}

		if ((
			 //   _stateMachine.State == PlayerStates.Jump ||
		     _stateMachine.State == PlayerStates.DoubleJump
		     || _stateMachine.State == PlayerStates.InAir)&&
		    ((_controller.collisionState.right && _directionalInput.x > 0) ||
		     ( _controller.collisionState.left&& _directionalInput.x < 0))&& _velocity.y <=0)
		{
			_stateMachine.ChangeState(PlayerStates.WallSliding);
		}
		
		
	}

	/// <summary>
	/// 冲刺
	/// </summary>
	private void Dash()
	{
		_stateMachine.ChangeState(PlayerStates.Dashing);
		//无敌帧
	}

	/// <summary>
	/// 获取玩家输入信息，进行其他函数的调用
	/// </summary>
	private void GetPlayerInput()
	{
		if (_inControl)
		{

			_directionalInput = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));

			if (Input.GetKeyDown(KeyCode.Space) && _stateMachine.State != PlayerStates.DoubleJump)
			{
				//看是否在滑墙，如果在滑墙则玩家要进行蹬墙跳
				if (_stateMachine.State == PlayerStates.WallSliding )
				{
					WallJump();
				}
				//如果没有滑墙，就是基本的跳跃
				else
				{
					Jump();
				}
			}

			if (Input.GetKeyUp(KeyCode.Space))
			{
				OnJumpKeyUp();
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
			if ( (Mathf.Abs(_directionalInput.x) - 0.01f) > 0)
			{
				_stateMachine.ChangeState(PlayerStates.Run);
			}

		}
	
		private void Idle_Exit()
		{
			//Debug.Log("Idle State Exit.");
		}

		#endregion
	
//      Forget Jump State..
//		#region JumpState
//
//		private void Jump_Enter()
//		{
//			_animator.Play(Animator.StringToHash("Jump"));
//			_controller.collisionState.below = false;
//		}
//	
//		private void Jump_Update()
//		{
//			if (_playerVelocityY < -_maxDownYSpeed)
//			{
//				_playerVelocityY = -_maxDownYSpeed;
//			}
//			
//			if (_controller.isGrounded)
//			{
//				ResetJumpPoint();
//			}
//
//			//处理高低跳
//			if (!Input.GetKeyUp(KeyCode.Space)) return;
//			
//			if (_playerVelocityY > _minJumpVelocity)
//			{
//				_playerVelocityY = _minJumpVelocity;
//			}
//		}
//	
//		private void Jump_Exit()
//		{
//			//Debug.Log("Jump Exit");
//		}
//
//		#endregion
	
	//forget this state
//		#region FallState
//		
//		private void Fall_Enter()
//		{
//			_animator.Play(Animator.StringToHash("Jump"));
//		}
//		
//		private void Fall_Update()
//		{
//			
//		}
//		
//		private void Fall_Exit()
//		{
//				
//		}
//		
//		#endregion
//		
	
		#region InAirState
			
		private void InAir_Enter()
		{
			Debug.Log("In air");
			//如果在空中，则自动认为主角已经失去一次跳跃机会
			_animator.Play(Animator.StringToHash("Jump"));
			_canJump = _jumpPoint - 1;
		}
			
		private void InAir_Update()
		{
			if (_playerVelocityY < -_maxDownYSpeed)
			{
				_playerVelocityY = -_maxDownYSpeed;
			}
			
			if (_controller.isGrounded)
			{
				ResetJumpPoint();
			}

			//处理高低跳
			if (!Input.GetKeyUp(KeyCode.Space)) return;
			
			if (_playerVelocityY > _minJumpVelocity)
			{
				_playerVelocityY = _minJumpVelocity;
			}
				
		}
			
		private void InAir_Exit()
		{
					
		}
			
		#endregion
		
		#region RunState
	
		private void Run_Enter()
		{
			_animator.Play(Animator.StringToHash("Move"));
		}
	
		private void Run_Update()
		{
			if (Mathf.Abs(_directionalInput.x) - 0.01f < 0) 
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
			_animator.Play(Animator.StringToHash("Jump"));
		}
	
		private void DoubleJump_Update()
		{
			if (_playerVelocityY < -_maxDownYSpeed)
			{
				_playerVelocityY = -_maxDownYSpeed;
			}
			
			if (_controller.isGrounded)
			{
				ResetJumpPoint();
			}

			//处理高低跳
			if (!Input.GetKeyUp(KeyCode.Space)) return;
			
			if (_playerVelocityY > _minJumpVelocity)
			{
				_playerVelocityY = _minJumpVelocity;
			}
		}
	
		private void DoubleJump_Exit()
		{
			
		}
	
		#endregion

		#region WallSlidingState
	
		private float _wallSlidingTimer= 0f;
		//一个计时器来判断玩家是否离开蹬墙跳状态
		private float WallStickTime = 0.1f;
	
		private void WallSliding_Enter()
		{
			//Debug.Log("WallSliding.");
			_wallSlidingTimer = 0.0f;
		}
		
		private void WallSliding_Update()
		{
			//WallSlidingTimer += Time.deltaTime;

			//Debug.Log(_directionalInput.x * _wallDirX);			
			
			if (_velocity.y < -_wallSlideVelocity)
			{
				_velocity.y = -_wallSlideVelocity;
			}

			//如果玩家持续反方向按下按键或者不按键到达一定时间，则判断玩家脱离墙体
			if ((_directionalInput.x * _wallDirX) <= 0)
			    //|| !_wallSliding)
			{
				if (_wallSlidingTimer > WallStickTime)
				{
					//Debug.LogError("Go");
					_stateMachine.ChangeState(PlayerStates.InAir);
				}

				_wallSlidingTimer += Time.deltaTime;
				//Debug.LogError("Sticking");
				_directionalInput.x = 0;
			}
			
			if (_controller.isGrounded)
			{
				ResetJumpPoint();
			}
			
		}
		
		private void WallSliding_Exit()
		{
			_wallSlidingTimer = 0f;
			
		}
	
		#endregion

		#region WallJumpState

		private float _wallJumpTimer;
	
		private void WallJump_Enter()
		{
			Debug.Log("Wall Jump");
			_animator.Play(Animator.StringToHash("Jump"));
			_wallJumpTimer = _wallJumpTime;
			_inControl = false;
		}
		
		private void WallJump_Update()
		{
			if (_wallJumpTimer >0)
			{
				_wallJumpTimer -= Time.deltaTime;
			}
			else
			{
				_inControl = true;
				_stateMachine.ChangeState(PlayerStates.InAir);
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
