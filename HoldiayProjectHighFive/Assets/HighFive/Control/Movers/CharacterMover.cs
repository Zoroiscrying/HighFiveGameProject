using ReadyGamerOne.EditorExtension;
using UnityEngine;
using HighFive.StateMachine;

namespace HighFive.Control.Movers
{
    /// <summary>
    /// 玩家角色移动器
    /// </summary>
    public class CharacterMover:ActorMover
    {
        #region Character_特有接口

        #region Consts

        protected const int JumpPoint = 2;

        #endregion
        
        #region Editor Configurable Variables
	    
        //最低跳跃高度（短按跳跃键）
        [SerializeField] protected float minJumpHeight = .25f;
        //是否可以高跳（超级跳）
        [SerializeField] protected bool canHighJump;
        //是否可以加速（技能）
        [SerializeField] protected bool canAcceleration;
        //是否可以冲刺
        [SerializeField] protected bool canDash;
        //是否可以二段跳
        [SerializeField] protected bool canDoubleJump;
	
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
        [SerializeField] protected float dashTime = .5f;
        [SerializeField] protected float dashDistance = 8f;

        [Header("Wall")]
        //public float _wallStickTime = 2f;//暂时不用
        [SerializeField] protected float wallSlideVelocity = 2f;
        [SerializeField] protected float wallJumpTime = .25f;
        [SerializeField] protected Vector2 wallJumpClimb;
        [SerializeField] protected Vector2 wallJumpNormal;

        [SerializeField] protected int canJumpTime = JumpPoint;

        [SerializeField] protected bool inControl = true;
    

        [Header("动画名字")] 
        public AnimationNameChooser idleAniName;
    //	public AnimationNameChooser walkAniName;
        public AnimationNameChooser jumpAniName;
        public AnimationNameChooser runAniName;
        public AnimationNameChooser dashAniName;
	
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

        public enum PlayerState
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
        
        public virtual float VelocityX { get; set; }
        public virtual float VelocityY { get; set; }        
        

        #endregion

    }
}