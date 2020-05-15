using System;
using HighFive.Control.Movers.Interfaces;
using HighFive.Others;
using UnityEngine;

namespace HighFive.Control.Movers
{
    /// <summary>
    /// 角色移动器
    /// 相对于BaseMover添加了基于跳跃高度和跳跃时间调整重力的办法
    /// 添加角色的移动速度；角色的输入（MoverInput）会影响角色实际移动速度
    /// 添加角色的动画控制
    /// </summary>
    [RequireComponent(typeof(Animator))][RequireComponent(typeof(SpriteRenderer))]
    public class ActorMover : BaseMover,IActorBaseControl
    {
        //暂时不知道放到哪里

        #region Rendering Relevant

        private SpriteRenderer _spriteRenderer;

        #endregion
        
        
        #region IMover2D

        public override float GravityScale
        {
            // do something;
            get => default;
            set => gravityScale = value;
        }

        public override float Gravity
        {
            get => default;
            set => throw new Exception("Actor不允许直接修改Gravity属性，请调节跳跃相关属性或者GravityScale.");
        }

        #endregion
        
        #region Actor_特有接口和属性

        [Header("PreciseMovementControl")] [SerializeField]
        private float accelerationTimeAirborne = .2f;

        [SerializeField] protected float accelerationTimeGrounded = .1f;
        [Space(5)] [SerializeField] protected float timeToJumpApex = .4f;
        [SerializeField] protected float maxJumpHeight = 1f;
        [Space(5)] [SerializeField] protected float runSpeed = 8f;
        [Space(5)] [SerializeField] protected float horizontalSpeedMultiplier = 1f;
        [SerializeField] protected float verticalSpeedMultiplier = 1f;
        [SerializeField] protected int faceDir = 1;//弃用（改用CollisionState.faceDir）
        [Header("Animation Control")] 
        public GameAnimator animator;
        [Header("Other")] public bool rayCastDebug = false;

        /// <summary>
        /// 角色的横向输入方向（1右-1左0为无输入）
        /// </summary>
        public int NormalizedInputDirX
        {
            get
            {
                if (moverInput.x > 0)
                {
                    return 1;
                }

                if (moverInput.x < 0)
                {
                    return -1;
                }

                return 0;
            }
        }

        /// <summary>
        /// 角色的面朝方向，右 1 ，左 - 1
        /// </summary>
        public virtual int FaceDir
        {
            get => collisionState.faceDir;
            set=>throw new Exception("Face FaceDir Cannot be changed by other code.");
        }

        /// <summary>
        /// 判断角色是否在平台边缘
        /// </summary>
        protected bool IsAtCorner
        {
            //判断到达边角的条件：横向射线和竖向射线
            get
            {
                //速度足够大，才进行边缘检测
                if (Mathf.Abs(velocity.x) - 0.01f >= 0)
                {
                    //横向射线检测
                    var rayDistance = 2 * SkinWidth;
                    var rayDirectionDownWard = Vector2.down;
                    var initialRayOriginL = _raycastOrigins.bottomLeft - new Vector2(SkinWidth, 0);
                    var initialRayOriginR = _raycastOrigins.bottomRight + new Vector2(SkinWidth, 0);
                    RaycastHit2D hit;

                    if (collisionState.right)
                    {
//					Debug.Log("Right: " + _controller.collisionState.right + "NormalizedX: " + _normalizedDirX);
//					Debug.Log("Right Collision");
                        return true;
                    }

                    if (collisionState.left)
                    {
//					Debug.Log("Left Collision");
                        return true;
                    }

                    //竖向射线检测
                    //向右走
                    if (velocity.x > 0)
                    {
                        hit = Physics2D.Raycast(initialRayOriginR, rayDirectionDownWard, rayDistance,
                            platformMask & ~ oneWayPlatformMask);
                    }
                    else
                    {
                        hit = Physics2D.Raycast(initialRayOriginL, rayDirectionDownWard, rayDistance,
                            platformMask & ~ oneWayPlatformMask);
                    }

                    if (!hit)
                    {
//					Debug.Log("Down Below no collision");
                        return true;
                    }
                }

                return false;
            }
        }

        private float _movementDampingHorizontal;
        private float _movementDampingVertical;

        protected float maxJumpVelocity;

        // 速度的缩放，现已整合在BaseMover中，为一二维向量。
        // 属性名为VelocityMultiplier

        /// <summary>
        /// TODO:是否忽略主观移动
        /// </summary>
        public bool IgnoreMoverInput { get; set; } = false;

        /// <summary>
        /// 控制是否可以移动的开关
        /// </summary>
        /// <param name="arg"></param>
        public virtual void SetMovable(bool arg)
        {
            this.canMove = arg;
        }

        /// <summary>
        /// 计算有关跳跃的各种变量，比如重力、跳起的速度
        /// </summary>
        protected virtual void CalculateGravityNVelocity()
        {
            gravity = (2 * maxJumpHeight) / Mathf.Pow(timeToJumpApex, 2);
            maxJumpVelocity = Mathf.Abs(gravity) * timeToJumpApex;
        }
        
        /// <summary>
        /// Actor的速度计算开始计算MoverInput对速度造成的影响，影响因子分别为RunSpeed、SpeedMultiplier和damp系数
        /// </summary>
        protected override void CalculateVelocity()
        {
            if (!IgnoreMoverInput)
            {
                var targetVelocityX = moverInput.x * runSpeed * horizontalSpeedMultiplier;
                // apply horizontal animationSpeed smoothing it. dont really do this with Lerp. Use SmoothDamp or something that provides more control
                //var smoothedMovementFactor = _controller.IsGrounded ? movementDamping : inAirDamping; // how fast do we change direction?
                velocity.x = Mathf.SmoothDamp(velocity.x, targetVelocityX, ref _movementDampingHorizontal,
                    (collisionState.below) ? accelerationTimeGrounded : accelerationTimeAirborne);
                // apply gravity before moving
                if (canMoveVertically)
                {
                    var targetVelocityY = moverInput.y * runSpeed * verticalSpeedMultiplier;
                    velocity.y = Mathf.SmoothDamp(velocity.y, targetVelocityY, ref _movementDampingVertical,
                        accelerationTimeAirborne);
                }
            }
            velocity.y -= gravity * gravityScale * Time.fixedDeltaTime;
        }

        /// <summary>
        /// 这个函数控制了实体左右移动时的朝向
        /// </summary>
        private void AnimFaceDirControl()
        {
            var localScaleThisFrame = transform.localScale;
            if (NormalizedInputDirX == 1) //向右
            {
                _spriteRenderer.flipX = false;
                // if (transform.localScale.x < 0f)
                // {       
                //     _spriteRenderer.flipX = false;
                //     transform.localScale =
                //     new Vector3(-localScaleThisFrame.x, localScaleThisFrame.y, localScaleThisFrame.z);
                // }
            }
            else if (NormalizedInputDirX == -1) //向左
            {
                _spriteRenderer.flipX = true;
                // if (transform.localScale.x > 0f)
                //     transform.localScale =
                //         new Vector3(-localScaleThisFrame.x, localScaleThisFrame.y, localScaleThisFrame.z);
            }
        }

        // 开放给外部使用的操纵Actor进行移动的接口，包括移动、跳跃和巡逻函数。
        // 需要注意的是本Actor类并不进行计时器控制，更高级的移动方法请调用ActorMovementController类中的函数
        #region Actor Movement Ctrl

        //-----------移动函数------------
        /// <summary>
        /// 控制Actor是否向右移动
        /// </summary>
        /// <param name="moveRight">是否向右移动</param>
        public void MoveHorizontally(bool moveRight)
        {
            if (moveRight)
            {
                moverInput.x = 1.0f;
            }
            else
            {
                moverInput.x = -1.0f;
            }
        }

        /// <summary>
        /// 反转Mover的横向移动输入
        /// </summary>
        public void ReverseMovementInputX()
        {
            moverInput.x = -moverInput.x;
        }

        /// <summary>
        /// 反转Mover的纵向移动输入
        /// </summary>
        public void ReverseMovementInputY()
        {
            moverInput.y = -moverInput.y;
        }

        /// <summary>
        /// 向Target方向移动，可以控制自己y轴移动速度的Actor（比如浮游物）会同时更改x、y方向的速度
        /// 无法控制自己y轴移动速度的Actor，则只能改变自己在x轴方向的移动。
        /// </summary>
        /// <param name="target">Target的世界2维xy坐标</param>
        public void MoveToward(Vector2 target)
        {
            var posVe2 = new Vector2(this.Position.x, this.Position.y);
            var dir = target - posVe2;
            moverInput = dir.normalized;
            moverInput.x = Mathf.Sign(moverInput.x);
            moverInput.y = Mathf.Sign(moverInput.y);
        }

        /// <summary>
        /// 停止x轴的Actor输入
        /// </summary>
        public void StopHorizontallyInput()
        {
            moverInput.x = 0;
        }

        /// <summary>
        /// 停止y轴的Actor输入
        /// </summary>
        public void StopVerticallyInput()
        {
            moverInput.y = 0;
        }

        /// <summary>
        /// 停止输入控制的位移
        /// </summary>
        public void StopMoverInput()
        {
            StopHorizontallyInput();
            StopVerticallyInput();
        }

        public float VelocityX
        {
            get => this.velocity.x;
            set => this.velocity.x = value;
        }
        public float VelocityY 
        {
            get => this.velocity.y;
            set => this.velocity.y = value;
        }
        
        
        public virtual bool IsGrounded=>collisionState.below;   

        /// <summary>
        /// 根据传入的击退方向，进行方向上的速度变化
        /// </summary>
        /// <param name="hitDir">带有Actor接受到的打击的方向信息和大小信息的向量</param>
        /// <param name="multiplier">用于影响击退效果的multiplier</param>
        public void ChangeVelBasedOnHitDir(Vector2 hitDir, float multiplier = 1)
        {
            this.velocity = hitDir * multiplier;
        }

        #endregion

        #endregion

        #region Monobehavior

        protected override void Awake()
        {
            base.Awake();
            _spriteRenderer = this.GetComponent<SpriteRenderer>();
            animator = GameAnimator.GetInstance(GetComponent<Animator>());
            CalculateGravityNVelocity();
            MoveHorizontally(true);
        }

        protected override void Update()
        {
            base.Update();
            AnimFaceDirControl();
        }

        protected override void FixedUpdate()
        {
            this.CalculateVelocity();
            
            if (canMove)
            {
                //Move the mover by its velocity * time.deltatime
                this.Move(new Vector2(velocity.x * velocityMultiplier.x, velocity.y * velocityMultiplier.y) *
                          Time.fixedDeltaTime);
            }
            
            CheckCollisions();
        }

        #endregion
    }
}