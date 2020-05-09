using UnityEngine;
using ReadyGamerOne.EditorExtension;
using ReadyGamerOne.Common;
using ReadyGamerOne.Script;
using HighFive.Const;
using HighFive.Global;
using HighFive.StateMachine;
namespace HighFive.Control.Movers
{
    /// <summary>
    /// 玩家角色移动器
    /// </summary>
    public class CharacterMover:AIActorMover,ICharacterMoverControl
    {
        #region MoverOverride
        protected override void CalculateGravityNVelocity()
        {
            base.CalculateGravityNVelocity();
            var minGravity = -(2 * minJumpHeight) / Mathf.Pow(timeToJumpApex, 2);
            _minJumpVelocity = Mathf.Abs(minGravity) * timeToJumpApex;
            var highJumpGravity = -(2 * highJumpHeight) / Mathf.Pow(timeToJumpApex, 2);
            _highJumpVelocity = Mathf.Abs(highJumpGravity) * timeToJumpApex;
        }

        #endregion
        
        #region Character_特有接口、函数和属性

        #region Consts

        protected const int AirJumpPoint = 1;

        #endregion
        
        #region Editor Configurable Variables
	    
        //最低跳跃高度（短按跳跃键）
        [SerializeField] protected float minJumpHeight = .25f;
        //是否可以高跳（超级跳）
        [SerializeField] protected bool canHighJump;
        //是否可以加速（技能）
        [SerializeField] protected bool canAcceleration;
	
        public float PlayerVelocityX
        {
            get => velocity.x;
            set => velocity.x = value;
        }
        public float PlayerVelocityY
        {
            get => velocity.y;
            set => velocity.y = value;
        }
        
        [Space(10)]
        //上升时的最大速度
        [SerializeField] protected float maxUpYSpeed = 999.0f;
        //下降时的最大速度
        [SerializeField] protected float maxDownYSpeed = 9.0f;
	
	
        [Header("HighJump")]
        [SerializeField] protected float highJumpHeight = 6.0f;
        //原有基础上增加的速度
        [Header("Acceleration")]
        [SerializeField] protected float accelerationSpeed = 2.0f;
        [SerializeField] protected float accelerationTime = 3.0f;
	
        [Header("Dash")]
        //是否可以冲刺
        [SerializeField] protected bool canDash;
        [SerializeField] protected float dashTime = .5f;
        [SerializeField] protected float dashDistance = 8f;

        [Header("Wall")]
        //public float _wallStickTime = 2f;//暂时不用
        [SerializeField] protected float wallSlideVelocity = 2f;
        [SerializeField] protected float wallJumpTime = .25f;
        [SerializeField] protected Vector2 wallJumpClimb;
        [SerializeField] protected Vector2 wallJumpNormal;

        [Header("Jump")]
        [SerializeField] protected bool canAirJump;
        [SerializeField] protected int airJumpTime = AirJumpPoint;
        [SerializeField] protected bool inControl = true;
    

        [Header("动画名字")] 
        public AnimationNameChooser idleAniName;
    //	public AnimationNameChooser walkAniName;
        public AnimationNameChooser jumpAniName;
        public AnimationNameChooser runAniName;
        public AnimationNameChooser dashAniName;

        [SerializeField]private PlayerState DebugStateChecker;
        
        #endregion
        
        #region Private Variables
        
        private StateMachine<PlayerState> _stateMachine;
        //timer
        //acceleration
        private float _aclratnTimer = 0.0f;
        //highjump
        private float _highJumpVelocity;
        //jump velocity
        private float _minJumpVelocity;
        //dash
        private float _dashVelocity;
        //wall sliding
        private bool _wallSliding;
        //running on wall
        private bool _runningOnWall;
        private int _wallDirX;
        private float _wallJumpDisableInputTimer;
        private bool _standingOnMovingPlatform = false;
        private bool _afterSuperUpZ = false;
	
        #endregion
        
        #region Struct, Class, Enums

        private enum PlayerState
        {
            Idle,
            Run,
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
        
        // 角色的XY方向速度已经在ActorMover中实现
        // public virtual float VelocityX { get; set; }
        // public virtual float VelocityY { get; set; }        
        
        public void SetDirectionalInput(Vector2 input)
        {
            moverInput = input;
        }
        
        private void OnJumpKeyDown()
        {
		
        }

        private void OnJumpKeyUp()
        {
		
        }
        
        private void CheckIsInAirState()
        {
            //在空中玩家只能施展二段跳
            if (!this.IsGrounded && _stateMachine.State != PlayerState.DoubleJump
                                        && _stateMachine.State != PlayerState.WallSliding&& _stateMachine.State != PlayerState.WallJump)
            {
                _stateMachine.ChangeState(PlayerState.InAir);
            }
        }
        
        private void ResetJumpPoint()
        {
            if (this.IsGrounded)
            {
                airJumpTime = AirJumpPoint;
            }
        }
        
        private void BasicStateCheck()
        {
            //AiIdle & run
            if (this.IsGrounded  && (_stateMachine.State != PlayerState.Dashing))//注意，是玩家没有输入左右键判断不是通过速度判断
            {
                //idle
                if (System.Math.Abs(moverInput.x) < 0.0001f)
                {
                    _stateMachine.ChangeState(PlayerState.Idle);				
                }
                //run
                if (System.Math.Abs(moverInput.x) > 0.0001f)
                {
                    _stateMachine.ChangeState(PlayerState.Run);
                }
            }
        }
        
        private void StartAcceleration(float time)
        {
            CEventCenter.BroadMessage(Message.M_ExitSuper);
            MainLoop.Instance.ExecuteLater(() => runSpeed -= accelerationSpeed, time);
            runSpeed += accelerationSpeed;
        }
        
        /// <summary>
        /// 玩家按下跳跃键后触发
        /// </summary>
        private void Jump()
        {
            if (GlobalVar.isSuper)
            {
                //High jump
//			Debug.Log("High Jump");
                _stateMachine.ChangeState(PlayerState.InAir);
                velocity.y = _highJumpVelocity;
                airJumpTime--;
                _afterSuperUpZ = false;
                CEventCenter.BroadMessage(Message.M_ExitSuper);
            }
            else if (_stateMachine.State == PlayerState.InAir && airJumpTime > 0)
            {
                velocity.y = maxJumpVelocity;
                airJumpTime--;
            }
            else
            {
	            _stateMachine.ChangeState(PlayerState.InAir);
	            velocity.y = maxJumpVelocity;
            }
        }
        
        private void WallJump()
        {
            if (this.IfNearWall(velocity)!=0 && (
                //  _stateMachine.State == PlayerStates.Jump ||
                //_stateMachine.State == PlayerState.InAir || 
                _stateMachine.State == PlayerState.WallSliding
                                                               || _stateMachine.State == PlayerState.InAir) )
            {
                _wallDirX = this.IfNearWall(velocity);
                _stateMachine.ChangeState(PlayerState.WallJump);
                if ((this.collisionState.right && _wallDirX == 1)||(this.collisionState.left&&_wallDirX == -1))
                {
                    //参考蔚蓝的蹬墙跳设计，如果玩家按键朝向墙，则蹬墙跳力气更大一些
                    velocity.y = wallJumpClimb.y;
                    velocity.x = wallJumpClimb.x * -_wallDirX;
                    return;
                }
                //如果玩家按键无朝向，则蹬墙跳力气小一些。
                velocity.y = wallJumpNormal.y;
                velocity.x = wallJumpNormal.x * -_wallDirX;
            }
        }
        
        private void WallSlide()
        {
            if (this.collisionState.left)
            {
                _wallDirX = -1;
            }
            else if (this.collisionState.right)
            {
                _wallDirX = 1;
            }
            else if (!this.collisionState.right && !this.collisionState.left )
            {
                _wallDirX = 0;
            }

            if ((
                    //   _stateMachine.State == PlayerStates.Jump ||
                    _stateMachine.State == PlayerState.DoubleJump
                    || _stateMachine.State == PlayerState.InAir)&&
                ((this.collisionState.right && moverInput.x > 0) ||
                 ( this.collisionState.left&& moverInput.x < 0))&& velocity.y <=0)
            {
                _stateMachine.ChangeState(PlayerState.WallSliding);
            }
        }
        
        private void Dash()
        {
            if (canDash)
            {
                _stateMachine.ChangeState(PlayerState.Dashing);
            }
            //无敌帧
        }
        
        private void GetPlayerInput()
        {
            if (inControl)
            {
                moverInput = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));

                if (Input.GetKeyDown(KeyCode.Space))
                {
	                //看是否在滑墙，如果在滑墙则玩家要进行蹬墙跳
                    if (_stateMachine.State == PlayerState.WallSliding)
                    {
                        WallJump();
                    }
                    //如果没有滑墙，就是基本的跳跃
                    else if (this.IsGrounded)
                    {
	                    Debug.Log("Jumped!");
	                    Jump();
                    }
                    else if (airJumpTime > 0)
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
                moverInput = new Vector2(0,0);
            }
        }
        

        #endregion
        
        #region Monobehavior

        protected override void Awake()
        {
            base.Awake();
            _stateMachine = StateMachine<PlayerState>.Initialize(this,PlayerState.Idle);
        }

        protected override void Update()
        {
	        base.Update();
            //灵力归元加速
            if (GlobalVar.isSuper)
            {
                if (Input.GetKeyUp(KeyCode.Z))
                {
                    _afterSuperUpZ = true;
                }
                if (_afterSuperUpZ && Input.GetKeyDown(KeyCode.Z))
                {
                    Debug.Log("加速");
                    _afterSuperUpZ = false;
                    StartAcceleration(accelerationTime);
                    CEventCenter.BroadMessage(Message.M_ExitSuper);
                }
            }

            CheckIsInAirState();
		
            WallSlide();
		
            ResetJumpPoint();
		
            GetPlayerInput();
		
            BasicStateCheck();

            CalculateGravityNVelocity();

            DebugStateChecker = _stateMachine.State;
        }

        #endregion
        
        #region StateCallBacks

		#region AiIdle

		private float _positionYLastFrame;
	
		private void Idle_Enter()
		{
			_positionYLastFrame = this.transform.position.y;
			animator.Play(Animator.StringToHash(idleAniName.StringValue));
		}
	
		private void Idle_Update()
		{
			if ( (Mathf.Abs(moverInput.x) - 0.001f) > 0)
			{
				_stateMachine.ChangeState(PlayerState.Run);
			}

		}
	
		private void Idle_Exit()
		{
			//Debug.Log("AiIdle State Exit.");
		}

		#endregion

		#region InAirState
			
		private void InAir_Enter()
		{
			//如果在空中，则自动认为主角已经失去一次跳跃机会
			animator.Play(Animator.StringToHash(jumpAniName.StringValue));
			//_canJump = _jumpPoint - 1;
		}
			
		private void InAir_Update()
		{
			if (PlayerVelocityY < -maxDownYSpeed)
			{
				PlayerVelocityY = -maxDownYSpeed;
			}
			
			if (this.IsGrounded)
			{
				ResetJumpPoint();
			}

			//处理高低跳
			if (!Input.GetKeyUp(KeyCode.Space)) return;
			
			if (PlayerVelocityY > _minJumpVelocity)
			{
				PlayerVelocityY = _minJumpVelocity;
			}
				
		}
			
		private void InAir_Exit()
		{
					
		}
			
		#endregion
		
		#region RunState
	
		private void Run_Enter()
		{
			animator.Play(Animator.StringToHash(runAniName.StringValue));
		}
	
		private void Run_Update()
		{
			if (Mathf.Abs(moverInput.x) - 0.01f < 0) 
			{
				_stateMachine.ChangeState(PlayerState.Idle);
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
			animator.Play(Animator.StringToHash(dashAniName.StringValue));
			_dashTimer = dashTime;
			inControl = false;
			_dashVelocity = dashDistance / dashTime;
			_dashDirX = faceDir;
		}
	
		private void Dashing_Update()
		{
			if (_dashDirX == 1 && this.collisionState.right)
			{
				_dashVelocity = 0;
			}
			else if (_dashDirX == -1 && this.collisionState.left)
			{
				_dashVelocity = 0;
			}
			
			if (_dashTimer > 0)
			{
				_dashTimer -= Time.deltaTime;
				velocity.x = _dashVelocity * _dashDirX;
			}
			else
			{
				_stateMachine.ChangeState(PlayerState.Idle);
			}
			
			
		}
	
		private void Dashing_Exit()
		{
			inControl = true;
		}
	
		#endregion

		#region DoubleJumpState
	
		private void DoubleJump_Enter()
		{
			//Debug.Log("DoubleJump Enter");
			animator.Play(Animator.StringToHash(jumpAniName.StringValue));
		}
	
		private void DoubleJump_Update()
		{
			if (PlayerVelocityY < -maxDownYSpeed)
			{
				PlayerVelocityY = -maxDownYSpeed;
			}
			
			if (this.IsGrounded)
			{
				ResetJumpPoint();
			}

			//处理高低跳
			if (!Input.GetKeyUp(KeyCode.Space)) return;
			
			if (PlayerVelocityY > _minJumpVelocity)
			{
				PlayerVelocityY = _minJumpVelocity;
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
			
			if (velocity.y < -wallSlideVelocity)
			{
				velocity.y = -wallSlideVelocity;
			}

			//如果玩家持续反方向按下按键或者不按键到达一定时间，则判断玩家脱离墙体
			if ((moverInput.x * _wallDirX) <= 0)
			    //|| !_wallSliding)
			{
				if (_wallSlidingTimer > WallStickTime)
				{
					//Debug.LogError("Go");
					_stateMachine.ChangeState(PlayerState.InAir);
				}

				_wallSlidingTimer += Time.deltaTime;
				//Debug.LogError("Sticking");
				moverInput.x = 0;
			}
			
			if (this.IsGrounded)
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
//			Debug.Log("Wall Jump");
			animator.Play(Animator.StringToHash(jumpAniName.StringValue));
			_wallJumpTimer = wallJumpTime;
			inControl = false;
		}
		
		private void WallJump_Update()
		{
			if (_wallJumpTimer >0)
			{
				_wallJumpTimer -= Time.deltaTime;
			}
			else
			{
				inControl = true;
				_stateMachine.ChangeState(PlayerState.InAir);
			}
			
			if (this.IsGrounded)
			{
				ResetJumpPoint();
			}
			
		}
	
		private void WallJump_Exit()
		{
			//Debug.Log("Wall Jump Exit");
			inControl = true;
		}
	
		#endregion

	#endregion
        
    }
}