using System;
using HighFive.Others;
using UnityEngine;
using zoroiscrying;

namespace HighFive.Control.Movers
{
    /// <summary>
    /// 角色移动器
    /// 相对于BaseMover添加了基于跳跃高度和跳跃时间调整重力的办法
    /// 添加角色的移动速度；角色的输入（MoverInput）会影响角色实际移动速度
    /// 添加角色的动画控制
    /// </summary>
    public class ActorMover:BaseMover
    {
        
        #region IMover2D

        public override float GravityScale
        {
            // do something;
            get => default;
            set => gravityScale = value;
        }

        public override float Gravity
        {
            get=>default; 
            set=>throw new Exception("Actor不允许直接修改Gravity属性，请调节跳跃相关属性或者GravityScale.");
        }

        #endregion
        
        #region Actor_特有接口和属性

        [Header("PreciseMovementControl")] 
        [SerializeField]private float accelerationTimeAirborne = .2f;
        [SerializeField]private float accelerationTimeGrounded = .1f;
        [Space(5)] 
        [SerializeField]private float timeToJumpApex = .4f;
        [SerializeField]private float maxJumpHeight = 1f;
        [Space(5)] 
        [SerializeField]private float runSpeed = 8f;
        [Space(5)] 
        [SerializeField]private float horizontalSpeedMultiplier = 1f; 
        [SerializeField]private float verticalSpeedMultiplier = 1f;
        [Header("Animation Control")]
        public GameAnimator animator;
        
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
        public virtual int FaceDir { get; set; }

        /// <summary>
        /// 判断角色是否在平台边缘
        /// </summary>
        private bool IsAtCorner
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

        private float _movementDamping;
        
        protected float maxJumpVelocity;
        
        // 速度的缩放，现已整合在BaseMover中，为一二维向量。
        // 属性名为VelocityMultiplier

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
            gravity = -(2 * maxJumpHeight) / Mathf.Pow(timeToJumpApex, 2);
            maxJumpVelocity = Mathf.Abs(gravity) * timeToJumpApex;
        }

        protected override void CalculateVelocity()
        {
            base.CalculateVelocity();
            float targetVelocityX = moverInput.x * runSpeed * horizontalSpeedMultiplier;
            // apply horizontal animationSpeed smoothing it. dont really do this with Lerp. Use SmoothDamp or something that provides more control
            //var smoothedMovementFactor = _controller.isGrounded ? movementDamping : inAirDamping; // how fast do we change direction?
            velocity.x = Mathf.SmoothDamp(velocity.x, targetVelocityX, ref _movementDamping,
                (collisionState.below) ? accelerationTimeGrounded : accelerationTimeAirborne);
            // apply gravity before moving
        }

        /// <summary>
        /// 这个函数控制了实体左右移动时的朝向
        /// </summary>
        private void AnimFaceDirControl()
        {
            if (NormalizedInputDirX == 1) //向右
            {
                //Debug.Log("Turn right!");
                FaceDir = 1;
                if (transform.localScale.x < 0f)
                    transform.localScale =
                        new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);
            }
            else if (NormalizedInputDirX == -1) //向左
            {
                FaceDir = -1;
                if (transform.localScale.x > 0f)
                    transform.localScale =
                        new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);
            }
        }
        
        #endregion


        #region Monobehavior

        protected override void Awake()
        {
            base.Awake();
            animator = GameAnimator.GetInstance(GetComponent<Animator>());
            CalculateGravityNVelocity();
        }

        protected override void Update()
        {
            base.Update();
            AnimFaceDirControl();
        }



        #endregion
    }
}