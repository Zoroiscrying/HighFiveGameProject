using HighFive.Control.Movers.Interfaces;
using UnityEngine;

namespace HighFive.Control.Movers
{
    public class AIActorMover : ActorMover, IActorMoveAIControl
    {
        #region AIActorMovementControl相关属性

        [Header("AutoPatrol")] 
        [SerializeField] protected int actorPatrolType;
        [SerializeField] protected bool isPatrolling = false;
        /// <summary>
        /// Mover巡逻过程中停下后等待的时间
        /// </summary>
        [SerializeField] protected float patrolStopTime = 1.0f;
        private float _patrolStopTimer = 0.0f;
        private Vector2 _patrolDirectionalInput = new Vector2(1, 0);
        private float _distanceCounter = 0;
        private float _patrolDistance = 0.0f;
        private bool _stopWhenDistanceSatisfied = false;

        public virtual float PatrolStopTime { get=>patrolStopTime; set=>patrolStopTime = value; }
        public virtual bool PatrolAllTheTime { get=>!_stopWhenDistanceSatisfied; set=>_stopWhenDistanceSatisfied=!value; }

        [Header("Auto Jump")] 
        [SerializeField] protected bool isJumping = false;
        [SerializeField] protected Vector2 jumpForce = new Vector2(1, 1);
        [SerializeField] protected bool autoJump = false;
        [SerializeField] protected float jumpStopTime = 1.0f;
        [SerializeField] protected int jumpsCountMax = 1;
        private float _jumpStopTimer = 0.0f;
        private int _jumpsCount = 0;

        public virtual float JumpStopTime { get=>jumpStopTime; set=>jumpStopTime=value; }
        public virtual bool JumpAllTheTime { get=>autoJump; set=>autoJump=value; }

        #endregion

        #region ActorMoverAIControl接口实现--巡逻

        /// <summary>
        /// 停止巡逻
        /// </summary>
        public void StopPatrolling()
        {
            isPatrolling = false;
        }

        /// <summary>
        /// 开始巡逻的默认函数，默认向右开始巡逻
        /// </summary>
        public void StartAutoPatrolling()
        {
            StartPatrolling(new Vector2(1, 0));
        }


        /// <summary>
        /// 开始巡逻
        /// </summary>
        /// <param name="dir">方向</param>
        /// <param name="patrolStopTimeS">停下时等待的时间</param>
        /// <param name="distance">向一个方向巡逻的距离</param>
        /// <param name="loopMove">是否循环巡逻</param>
        public void StartPatrolling(Vector2 dir, float patrolStopTimeS = 0.0f, float distance = float.MaxValue, bool loopMove = false)
        {
            StopPatrolling();
            //默认认为距离满足不停下以及距离最大
            _stopWhenDistanceSatisfied = !loopMove;
            _patrolDistance = distance;
            //设定站立时间
            this.patrolStopTime = patrolStopTimeS;
            //打开Update函数中的巡逻开关
            isPatrolling = true;
            //input赋值
            dir.x = Mathf.Sign(dir.x);
            if (this.canMoveVertically)
            {
                dir.y = Mathf.Sign(dir.y);
            }
            else
            {
                dir.y = 0;
            }

            _patrolDirectionalInput = dir;
        }
        

        /// <summary>
        /// 巡逻函数，在Update调用，不可被外部调用
        /// </summary>
        private void PatrolUpdate()
        {
            //到达边角后，开启计时器（增加计时器），计时器到点，继续前进
            if (this.IsAtCorner)
            {
                //设置一个event
                //开启计时器
                _patrolStopTimer = 0.0f;
                _patrolDirectionalInput = -_patrolDirectionalInput;
                //Debug.Log("Hit Corner");
            }

            //站住时不运动
            if (_patrolStopTimer < patrolStopTime)
            {
                //站住
                moverInput.x = 0;
                _patrolStopTimer += Time.deltaTime;
            }
            else //不站着时运动
            {
                _distanceCounter += Mathf.Abs(runSpeed) * Time.fixedDeltaTime * _patrolDirectionalInput.magnitude;
                moverInput = _patrolDirectionalInput;
                //_velocity.x = Mathf.Abs(_runSpeed) * _movementMultiplier;			
                if (_distanceCounter > _patrolDistance)
                {
                    _distanceCounter = 0.0f;
                    if (_stopWhenDistanceSatisfied)
                    {
                        StopPatrolling();
                    }
                    else
                    {
                        _patrolStopTimer = 0.0f;
                        _patrolDirectionalInput = -_patrolDirectionalInput;
                    }
                }
            }
        }

        #endregion

        #region ActorMoverAIControl接口实现--跳跃

        private void JumpOnce(Vector2 force)
        {
            if (this.IsGrounded)
            {
                velocity.x = force.x;
                velocity.y = force.y;
            }
        }
        public void StopJumping()
        {
            _jumpsCount = 0;
            isJumping = false;
        }
        
        public void StartJumping(Vector2 force, float jumpStopTimeS = 0.0f, int jumpCountMaxS = 1, bool loopJump = false)
        {
            StopJumping();
            jumpsCountMax = jumpCountMaxS;
            this.jumpForce = force;
            autoJump = loopJump;
            isJumping = true;
        }

        public void JumpUpdate()
        {
            if (_jumpStopTimer <= jumpStopTime && this.IsGrounded)
            {
                _jumpStopTimer += Time.deltaTime;
                return;
            }
            
            //正常跳跃
            if (_jumpsCount < jumpsCountMax)
            {
                if (this.IsGrounded)
                {
                    JumpOnce(jumpForce);
                    _jumpsCount++;
                    _jumpStopTimer = 0.0f;
                }
            }
            else
            {
                if (autoJump)
                {
                    _jumpsCount = 0;
                }
                else
                {
                    StopJumping();
                }
            }
        }

        #endregion

        #region ActorMoverAIControl接口实现--移动

        #endregion

        #region Monobehavior

        protected override void Awake()
        {
            base.Awake();
            // StartPatrolling(Vector2.right,0f,3f,true);
            // StartJumping(new Vector2(0,5),0f,1,true);
        }

        protected override void FixedUpdate()
        {
            base.FixedUpdate();
            if (isPatrolling)
            {
                PatrolUpdate();
            }

            if (isJumping)
            {
                JumpUpdate();
            }
        }

        #endregion
    }
}