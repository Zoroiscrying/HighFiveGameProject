using System;
using System.Collections.Generic;
using ReadyGamerOne.Rougelike.Mover;
using UnityEngine;

namespace HighFive.Control.Movers
{
    /// <summary>
    /// 移动器基类
    /// 负责每帧通过velocity进行移动
    /// 通过Raycast进行碰撞（如果碰撞则调整velocity）和trigger检测
    /// 
    /// </summary>
    public class BaseMover:MonoBehaviour,IMover2D
    {
        
        #region Raycast Relevant Attributes and Functions
        /// <summary>
        /// 射线检测相关属性和函数，但不包括update和start函数的实现
        /// </summary>
        [SerializeField] [Range( 0.001f, 0.3f )]
        //Skin width, Mover和环境开始产生碰撞的最小间隔
        private float _skinWidth = 0.02f;
        public float SkinWidth
        {
            get { return _skinWidth; }
            set
            {
                _skinWidth = value;
                RecalculateDistanceBetweenRays();//重新设置skinWidth后需要重新计算rayCast
            }
        }
        //纵向和横向的射线检测数量
        [SerializeField][Range( 2, 20 )]
        private int totalHorizontalRays = 8;
        [SerializeField][Range( 2, 20 )]
        private int totalVerticalRays = 4;
        //纵向和横向的各个射线检测间隔
        private float _verticalDistanceBetweenRays;
        private float _horizontalDistanceBetweenRays;

        public struct CharacterRaycastOrigins //角色的检测射线
        {
            public Vector2 topLeft;
            public Vector2 bottomRight;
            public Vector2 bottomLeft;
            public Vector2 topRight;
        }

        private CharacterRaycastOrigins _raycastOrigins;
        private BoxCollider2D boxCollider;
        
        // we use this flag to mark the case where we are travelling up a slope and we modified our delta.y to allow the climb to occur.
        // the reason is so that if we reach the end of the slope we can make an adjustment to stay grounded
        private bool _isGoingUpWall = false;
        
        /// <summary>
        /// 计算Ray之间的distance
        /// </summary>
        public void RecalculateDistanceBetweenRays()//高度和宽度考虑到colliderSize * localScale - 2个皮肤宽度
        {
            // figure out the distance between our rays in both directions
            // horizontal
            var colliderUseableHeight = boxCollider.size.y * Mathf.Abs( transform.localScale.y ) - ( 2f * _skinWidth );
            _verticalDistanceBetweenRays = colliderUseableHeight / ( totalHorizontalRays - 1 );

            // vertical
            var colliderUseableWidth = boxCollider.size.x * Mathf.Abs( transform.localScale.x ) - ( 2f * _skinWidth );
            _horizontalDistanceBetweenRays = colliderUseableWidth / ( totalVerticalRays - 1 );
        }

        /// <summary>
        /// 初始化RayCast起始点本地坐标
        /// </summary>
        public void InitializeRayCastOrigins()
        {
            // our raycasts need to be fired from the bounds inset by the skinWidth
            var modifiedBounds = boxCollider.bounds;
            modifiedBounds.Expand( -2f * _skinWidth );

            _raycastOrigins.topLeft = new Vector2( modifiedBounds.min.x, modifiedBounds.max.y );
            _raycastOrigins.bottomRight = new Vector2( modifiedBounds.max.x, modifiedBounds.min.y );
            _raycastOrigins.bottomLeft = new Vector2(modifiedBounds.min.x,modifiedBounds.min.y);
            _raycastOrigins.topRight = new Vector2(modifiedBounds.max.x,modifiedBounds.max.y);
        }
        
        [System.Diagnostics.Conditional("DEBUG_CC2D_RAYS")]
        public void DrawRay(Vector3 start, Vector3 dir, Color color)
        {
            Debug.DrawRay(start,dir,color);
        }
        
        /// <summary>
        /// Debug function，画出角色的碰撞检测线
        /// </summary>
        [System.Diagnostics.Conditional( "DEBUG_CC2D_RAYS" )]
        private void DrawRay()
        {
            //horizontal
            for (int i = 0; i < totalHorizontalRays; i++)
            {
                Vector2 rayOrigin =  _raycastOrigins.bottomLeft;
                rayOrigin += Vector2.up * _verticalDistanceBetweenRays *i;
                Debug.DrawRay( rayOrigin, Vector2.up, Color.red );                                                                                             	
            }
            for (int i = 0; i < totalHorizontalRays; i++)
            {
                Vector2 rayOrigin = _raycastOrigins.bottomRight ;
                rayOrigin += Vector2.up * _verticalDistanceBetweenRays *i;
                Debug.DrawRay( rayOrigin, Vector2.down, Color.red );                                                                                                 	
            }
            //vertical
            for (int i = 0; i < totalVerticalRays; i++)
            {
                Vector2 rayOrigin =  _raycastOrigins.topLeft;
                rayOrigin += Vector2.right * _horizontalDistanceBetweenRays *i;
                Debug.DrawRay( rayOrigin, Vector2.up, Color.red );                                                                                             	
            }
            for (int i = 0; i < totalVerticalRays; i++)
            {
                Vector2 rayOrigin = _raycastOrigins.bottomLeft ;
                rayOrigin += Vector2.right * _horizontalDistanceBetweenRays *i;
                Debug.DrawRay( rayOrigin, Vector2.down, Color.red );                                                                                                 	
            }
        }
        
        

        #endregion
        
        #region IMover2D

        /// <summary>
        /// 重力，影响Mover的垂直速度
        /// </summary>
        [SerializeField] private float gravity = 20f;
        public override float Gravity {
            get => gravity;
            set => gravity = value;
        }
        /// <summary>
        /// 速度，影响实际物体的移动
        /// </summary>
        [SerializeField]private Vector2 velocity;
        public virtual Vector2 Velocity
        {
            get => velocity;
            set => velocity = value;
        }
        /// <summary>
        /// 重力Scaler，影响重力改变速度的程度
        /// </summary>
        [SerializeField] private float gravityScale = 1.0f;
        public virtual float GravityScale { 
            get => gravityScale;
            set => gravityScale = value;
        }
        /// <summary>
        /// mask with all layers that the player should interact with
        /// </summary>
        public LayerMask platformMask = 0;

        /// <summary>
        /// mask with all layers that trigger events should fire when intersected
        /// </summary>
        public LayerMask triggerMask = 0;
        public virtual LayerMask ColliderLayers { 
            get => platformMask;
            set => platformMask = value;
        }
        public virtual LayerMask TriggerLayers
        {
            get => triggerMask;
            set => triggerMask = value;
        }
        public event Action<GameObject, ReadyGamerOne.Rougelike.Mover.TouchDir> eventOnColliderEnter;
        public event Action<GameObject, ReadyGamerOne.Rougelike.Mover.TouchDir> eventOnTriggerEnter;

        #endregion
        
        #region UnityCallBacks_子类如果想用的话一定要override父类的，自己另写的话父类就不会调用了

        protected virtual void Awake(){}

        protected virtual void Start(){}

        protected virtual void Update(){}

        protected virtual void FixedUpdate(){}

        protected virtual void OnTriggerEnter2D(Collider2D collider2D){}

        protected virtual void OnCollisionEnter2D(Collision2D collision2D){}        

        #endregion
        
        #region Movement Relevant Attributes and Functions

        [HideInInspector] public Vector2 MoverInput;

        /// <summary>
        /// Mover的碰撞状态
        /// </summary>
        public class MoverCollisionState2D 
        {
            public bool right;
            public bool left;
            public bool above;
            public bool below; //collision infos
            public bool becameGroundedThisFrame; //这一帧到达地面
            public bool wasGroundedLastFrame; //上一帧到达地面
            public bool fallingThroughPlatform;
            public float slopeAngle; //滑坡临街角度
            public int faceDir; //1 facing right , -1 facing left
            public Vector2 slopeNormal;

            public bool HasCollisionAround()
            {
                return below || right || left || above; //如果向下/向右/向左/向上中有一个为真，则表明有Collision
            }

            public void Reset()
            {
                right = left =
                    above = below = becameGroundedThisFrame  = false; //全部设置为false
                fallingThroughPlatform = false;
                slopeNormal = Vector2.zero;
                slopeAngle = 0f; //滑坡角度为0°
            }
        }
        //Events
        public event Action<RaycastHit2D> onControllerCollidedEvent; //如果碰撞会调用这个event
        public event Action<Collider2D> onTriggerEnterEvent; //如果TriggerEnter会调用这个event
        public event Action<Collider2D> onTriggerStayEvent; //如果TriggerStay会调用这个event
        public event Action<Collider2D> onTriggerExitEvent; //同理

        /// <summary>
        /// when true, one way platforms will be ignored when moving vertically for a single frame
        /// </summary>
        public bool ignoreOneWayPlatformsThisFrame; //这一帧忽略OneWayPlatform层，可以穿过

        /// <summary>
        /// mask with all layers that should act as one-way platforms. Note that one-way platforms should always be EdgeCollider2Ds. This is because it does not support being
        /// updated anytime outside of the inspector for now.
        /// </summary>
        [SerializeField] public LayerMask oneWayPlatformMask = 0;

        /// <summary>
        /// the max slope angle that the CC2D can climb
        /// </summary>
        /// <value>The slope limit.</value>
        [Range(0f, 90f)] public float slopeLimit = 30f;

        /// <summary>
        /// the threshold in the change in vertical movement between frames that constitutes jumping
        /// </summary>
        /// <value>The jumping threshold.</value>
        public float jumpingThreshold = 0.07f;

        private new Transform transform;
        private Rigidbody2D rigidBody2D;
        [SerializeField] private MoverCollisionState2D collisionState = new MoverCollisionState2D();
        public bool isGrounded
        {
            get { return collisionState.below; }
        }

        const float SkinWidthFloatFudgeFactor = 0.001f;
        
        RaycastHit2D _raycastHit;

        /// <summary>
        /// stores any raycast hits that occur this frame. we have to store them in case we get a hit moving
        /// horizontally and vertically so that we can send the events after all collision state is set
        /// </summary>
        List<RaycastHit2D> _raycastHitsThisFrame = new List<RaycastHit2D>(2);

        // we use this flag to mark the case where we are travelling up a slope and we modified our delta.y to allow the climb to occur.
        // the reason is so that if we reach the end of the slope we can make an adjustment to stay grounded

        /// <summary>
        /// 正确的控制移动应该是改变mover的velocity属性，直接调用move函数并不推荐。
        /// </summary>
        /// <param name="deltaMovement"></param>
        /// <param name="standingOnPlatform"></param>
        public void Move(Vector2 deltaMovement, bool standingOnPlatform = false)
        {
            Move(deltaMovement, Vector2.zero, standingOnPlatform);
        }
        
        /// <summary>
        /// attempts to move the character to position + deltaMovement. Any colliders in the way will cause the movement to
        /// stop when run into.
        /// </summary>
        /// <param CharacterName="deltaMovement">Delta movement.</param>
        public void Move(Vector2 deltaMovement, Vector2 input, bool standingOnPlatform = false)
        {
            InitializeRayCastOrigins();

            // save off our current grounded state which we will use for wasGroundedLastFrame and becameGroundedThisFrame
            collisionState.wasGroundedLastFrame = collisionState.below;
            // clear our state
            collisionState.Reset();
            _raycastHitsThisFrame.Clear();
            MoverInput = input;
            
//            
//            // first, we check for a slope below us before moving
//            // only check slopes if we are going down and grounded
//            if (deltaMovement.y < 0f && collisionState.wasGroundedLastFrame)
//            {
//               // DecendSlope(ref deltaMovement);
//            }

            if (deltaMovement.x != 0)
            {
                collisionState.faceDir = (int) Mathf.Sign(deltaMovement.x);
            }

            CheckHorizontalCollision(ref deltaMovement);

            if (deltaMovement.y != 0)
            {
                CheckVerticalCollision(ref deltaMovement);
            }

            //实际的移动在这里发生
            transform.Translate(deltaMovement, Space.World);

            // 计算速度
            if (Time.deltaTime > 0f)
                velocity = deltaMovement / Time.fixedDeltaTime;

            //如果移动前不在地面上，而移动后到达了地面，则把如下变量设为true
            if (!collisionState.wasGroundedLastFrame && collisionState.below)
                collisionState.becameGroundedThisFrame = true;

            // 委托事件调用
            if (onControllerCollidedEvent != null)
            {
                for (var i = 0; i < _raycastHitsThisFrame.Count; i++)
                {
                    onControllerCollidedEvent(_raycastHitsThisFrame[i]);
                }
            }

            if (standingOnPlatform && !collisionState.wasGroundedLastFrame)
            {
                collisionState.below = true;
                collisionState.becameGroundedThisFrame = true;
            }

            ignoreOneWayPlatformsThisFrame = false;
        }
        
        /// <summary>
        /// 获得角色中间脚下的平台Hit
        /// </summary>
        /// <returns></returns>
        public RaycastHit2D GetFirstCastHitBelow()
        {
            RaycastHit2D hit;
            var rayDistance = 2 * _skinWidth;
            var rayDirection = Vector2.down;
            var initialRayOrigin = (_raycastOrigins.bottomRight + _raycastOrigins.bottomLeft) / 2;

            var ray = new Vector2(initialRayOrigin.x, initialRayOrigin.y);
            hit = Physics2D.Raycast(ray, rayDirection * rayDistance, platformMask);

            return hit;
        }

        /// <summary>
        /// 检查Mover这一帧到下一帧是否在墙附近，1表示在右侧，-1表示在左侧，0表示没有在墙附近
        /// </summary>
        /// <param name="deltaMovement"></param>
        /// <returns></returns>
        public int IfNearWall(Vector2 deltaMovement)
        {
            //var rayDistance = Mathf.Abs( deltaMovement.x ) + skinWidth;
            //TODO::这里如果出现问题可以改回去为2*skinwidth
            var rayDistance = Mathf.Abs( deltaMovement.x ) + _skinWidth;
            
            var rayDirectionR = Vector2.right;
            var initialRayOriginR = _raycastOrigins.bottomRight;

            var rayDirectionL = Vector2.left;
            var initialRayOriginL = _raycastOrigins.bottomLeft;
            RaycastHit2D hit;

            if (Mathf.Abs(deltaMovement.x) < _skinWidth)
            {
                rayDistance = 2 * _skinWidth;
            }

            for (var i = 2; i < 8; i++)
            {
                var ray = new Vector2(initialRayOriginR.x, initialRayOriginR.y + i * _verticalDistanceBetweenRays);

                //DrawRay(ray, rayDirectionR * rayDistance, Color.red);

                // if we are grounded we will include oneWayPlatforms only on the first ray (the bottom one). this will allow us to
                // walk up sloped oneWayPlatforms
                if (i == 0 && collisionState.wasGroundedLastFrame)
                    hit = Physics2D.Raycast(ray, rayDirectionR, rayDistance, platformMask);
                else
                    hit = Physics2D.Raycast(ray, rayDirectionR, rayDistance, platformMask & ~oneWayPlatformMask);

                if (hit)
                {
                    if (hit.distance <= rayDistance)
                    {
                        return 1;
                    }

                    // the bottom ray can hit a slope but no other ray can, i == 0 makes sure that this ray is the botton ray
                }
            }
            //todo::改进？？

            for (var i = 2; i < 8; i++)
            {
                var ray = new Vector2(initialRayOriginL.x, initialRayOriginL.y + i * _verticalDistanceBetweenRays);

                //DrawRay(ray, rayDirectionL * rayDistance, Color.red);

                // if we are grounded we will include oneWayPlatforms only on the first ray (the bottom one). this will allow us to
                // walk up sloped oneWayPlatforms
                if (i == 0 && collisionState.wasGroundedLastFrame)
                    hit = Physics2D.Raycast(ray, rayDirectionL, rayDistance, platformMask);
                else
                    hit = Physics2D.Raycast(ray, rayDirectionL, rayDistance, platformMask & ~oneWayPlatformMask);
                
                if (hit)
                {
                    if (hit.distance <= rayDistance)
                    {
                        return -1;
                    }

                    // the bottom ray can hit a slope but no other ray can, i == 0 makes sure that this ray is the botton ray
                }
            }
            
            //todo::改进？？

            return 0;
        }

         /// <summary>
        /// 负责Controller的横向碰撞检测
        /// </summary>
        void CheckHorizontalCollision(ref Vector2 deltaMovement)
        {
            var isGoingRight = collisionState.faceDir;
            var rayDistance = Mathf.Abs(deltaMovement.x) + _skinWidth;
            var rayDirection = (isGoingRight == 1) ? Vector2.right : -Vector2.right;
            var initialRayOrigin = (isGoingRight == 1) ? _raycastOrigins.bottomRight : _raycastOrigins.bottomLeft;

            if (Mathf.Abs(deltaMovement.x) < _skinWidth)
            {
                rayDistance = 2 * _skinWidth;
            }

            for (var i = 0; i < totalHorizontalRays; i++)
            {
                var ray = new Vector2(initialRayOrigin.x, initialRayOrigin.y + i * _verticalDistanceBetweenRays);

                DrawRay(ray, rayDirection * rayDistance, Color.red);

                // if we are grounded we will include oneWayPlatforms only on the first ray (the bottom one). this will allow us to
                // walk up sloped oneWayPlatforms
                if (i == 0 && collisionState.wasGroundedLastFrame)
                    _raycastHit = Physics2D.Raycast(ray, rayDirection, rayDistance, platformMask);
                else
                    _raycastHit = Physics2D.Raycast(ray, rayDirection, rayDistance, platformMask & ~oneWayPlatformMask);


                if (_raycastHit)
                {
//                    Debug.Log("Hit Wall");
                    //todo::如果横向检测出了问题，就来这里看看
                    if (_raycastHit.distance <= 0)
                    {
                        continue;
                    }

//                    float slopeAngle = Vector2.Angle(_raycastHit.normal, Vector2.up);
//
//                    // the bottom ray can hit a slope but no other ray can, i == 0 makes sure that this ray is the botton ray
//                    if (i == 0 && HandleHorizontalSlope(ref deltaMovement,
//                            Vector2.Angle(_raycastHit.normal, Vector2.up)))
//                    {
//                        _raycastHitsThisFrame.Add(_raycastHit);
//                        // if we weren't grounded last frame, that means we're landing on a slope horizontally.
//                        // this ensures that we stay flush to that slope
//                        if (!collisionState.wasGroundedLastFrame)
//                        {
//                            float flushDistance = Mathf.Sign(deltaMovement.x) * (_raycastHit.distance - skinWidth);
//                            deltaMovement.x += flushDistance;
//                        }
//
//                        break;
//                    }
//
                    // set our new deltaMovement and recalculate the rayDistance taking it into account
                    deltaMovement.x = (_raycastHit.distance - _skinWidth) * rayDirection.x;
                    rayDistance = _raycastHit.distance;

                    collisionState.left = isGoingRight == -1;
                    collisionState.right = isGoingRight == 1;
                    
                    //todo::这里可能会出问题
                    if (!_raycastHitsThisFrame.Contains(_raycastHit))
                    {
                        _raycastHitsThisFrame.Add(_raycastHit);
                        if (collisionState.left)
                        {
                            eventOnColliderEnter?.Invoke(_raycastHit.transform.gameObject, TouchDir.Left);
                        }
                        else if(collisionState.right)
                        {
                            eventOnColliderEnter?.Invoke(_raycastHit.transform.gameObject, TouchDir.Left);
                        }
                    }
                    // _raycastHitsThisFrame.Add(_raycastHit);
                    // we add a small fudge factor for the float operations here. if our rayDistance is smaller
                    // than the width + fudge bail out because we have a direct impact
                    if (rayDistance < _skinWidth + SkinWidthFloatFudgeFactor)
                        break;
                }
            }
        }


        /// <summary>
        /// 负责Controller的竖向碰撞检测
        /// </summary>
        /// <param name="deltaMovement"></param>
        void CheckVerticalCollision(ref Vector2 deltaMovement)
        {
            var isGoingUp = Mathf.Sign(deltaMovement.y); //if going up 1 ; if not -1
            var rayDistance =
                Mathf.Abs(deltaMovement.y) + _skinWidth; //the ray's original starting point is within the collider
            var rayDirection = isGoingUp == 1 ? Vector2.up : -Vector2.up;
            var initialRayOrigin = isGoingUp == 1 ? _raycastOrigins.topLeft : _raycastOrigins.bottomLeft;

            // apply our horizontal deltaMovement here so that we do our raycast from the actual position we would be in if we had moved
            initialRayOrigin.x += deltaMovement.x;

            if (Mathf.Abs(deltaMovement.y) < _skinWidth)
            {
                rayDistance = 2 * _skinWidth;
            }

            // if we are moving up, we should ignore the layers in oneWayPlatformMask
            var mask = platformMask;
            if ((isGoingUp == 1 && !collisionState.wasGroundedLastFrame) || ignoreOneWayPlatformsThisFrame)
                mask &= ~oneWayPlatformMask;

            for (var i = 0; i < totalVerticalRays; i++)
            {
                var ray = new Vector2(initialRayOrigin.x + i * _horizontalDistanceBetweenRays, initialRayOrigin.y);

                DrawRay(ray, rayDirection * rayDistance, Color.red);
                _raycastHit = Physics2D.Raycast(ray, rayDirection, rayDistance, mask); //fetch the hit information.
                if (_raycastHit) //if hit something
                {
                    if (_raycastHit.collider.CompareTag("Through"))
                    {
                        if (rayDirection.y == 1 || _raycastHit.distance <= 0)
                        {
                            Debug.Log("Vertical Distance <= 0.");
                            continue;
                        }

                        if (collisionState.fallingThroughPlatform && deltaMovement.y <= 0)
                        {
                            continue;
                        }
                    }

                    // set our new deltaMovement and recalculate the rayDistance taking it into account
                    deltaMovement.y = (_raycastHit.distance - _skinWidth) * rayDirection.y;
                    rayDistance = Mathf.Abs(deltaMovement.y); //get the distance between the ray point and the hit point

                    if (isGoingUp == 1)
                    {
                        collisionState.above = true;
                    }
                    else
                    {
                        collisionState.below = true;
                        //Debug.Log("Grounded.");
                    }

                    if (!_raycastHitsThisFrame.Contains(_raycastHit))
                    {
                        _raycastHitsThisFrame.Add(_raycastHit); //stores all the hits this frame.
                        if (collisionState.above)
                        {
                            eventOnColliderEnter?.Invoke(_raycastHit.transform.gameObject, TouchDir.Top);
                        }
                        else if(collisionState.below)
                        {
                            eventOnColliderEnter?.Invoke(_raycastHit.transform.gameObject, TouchDir.Bottom);
                        }
                    }

//                    // this is a hack to deal with the top of slopes. if we walk up a slope and reach the apex we can get in a situation
//                    // where our ray gets a hit that is less then skinWidth causing us to be ungrounded the next frame due to residual velocity.
//                    if (!(isGoingUp == 1) && deltaMovement.y > 0.00001f)
//                        collisionState.climbingSlope = true;

                    // we add a small fudge factor for the float operations here. if our rayDistance is smaller
                    // than the width + fudge bail out because we have a direct impact
                    
                    if (rayDistance < _skinWidth + SkinWidthFloatFudgeFactor)
                        break;
                }
            }
            
//
//            if (collisionState.climbingSlope)
//            {
//                float directionX = Mathf.Sign(deltaMovement.x);
//                rayDistance = Mathf.Abs(deltaMovement.x) + skinWidth;
//                Vector2 rayOrigin = ((directionX == -1) ? _raycastOrigins.bottomLeft : _raycastOrigins.bottomRight) +
//                                    Vector2.up * deltaMovement.y;
//                RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.right * directionX, rayDistance, platformMask);
//
//                if (hit)
//                {
//                    float slopeAngle = Vector2.Angle(hit.normal, Vector2.up);
//                    if (slopeAngle != collisionState.slopeAngle)
//                    {
//                        deltaMovement.x = (hit.distance - skinWidth) * directionX;
//                        collisionState.slopeAngle = slopeAngle;
//                        collisionState.slopeNormal = hit.normal;
//                    }
//                }
//            }
            
        }
        
        #endregion

        #region Monobehavior

        protected override void Awake()
        {
            base.Awake();
            // add our one-way platforms to our normal platform mask so that we can land on them from above
            
            // cache some components
            boxCollider = GetComponent<BoxCollider2D>();
            InitializeRayCastOrigins();
            transform = GetComponent<Transform>();
            rigidBody2D = GetComponent<Rigidbody2D>();
            // here, we trigger our properties that have setters with bodies
            SkinWidth = _skinWidth;
            
            // add our one-way platforms to our normal platform mask so that we can land on them from above
            platformMask |= oneWayPlatformMask;
            collisionState.faceDir = 1;
            
            // we want to set our CC2D to ignore all collision layers except what is in our triggerMask
            for (var i = 0; i < 32; i++)
            {
                // see if our triggerMask contains this layer and if not ignore it
                if (((triggerMask.value << i)) == 0)
                    Physics2D.IgnoreLayerCollision(gameObject.layer, i);
            }
        }

        protected override void FixedUpdate()
        {
            base.FixedUpdate();
            
            CalculateVelocity();
            
            //Move the mover by its velocity * time.deltatime
            this.Move(velocity*Time.fixedDeltaTime);
            
            CheckCollisions();
        }

        protected virtual void CalculateVelocity()
        {
            velocity.y -= gravity * GravityScale * Time.deltaTime;
        }

        protected virtual void CheckCollisions()
        {
            if (this.collisionState.right || this.collisionState.left)
            {
                velocity.x = 0;
            }

            if ((this.collisionState.below || this.collisionState.above) &&
                !this.collisionState.fallingThroughPlatform)
            {
                velocity.y = 0;
            }
        }

        protected override void Update()
        {
            base.Update();
            DrawRay();
            // if (Input.GetKeyDown(KeyCode.Space))
            // {
            //     this.velocity = new Vector2(2,20);
            // }
        }

        protected override void OnTriggerEnter2D(Collider2D col)
        {
            base.OnTriggerEnter2D(col);
            onTriggerEnterEvent?.Invoke(col);
        }


        public void OnTriggerStay2D(Collider2D col)
        {
            onTriggerStayEvent?.Invoke(col);
        }
        

        public void OnTriggerExit2D(Collider2D col)
        {
            onTriggerExitEvent?.Invoke(col);
        }

        #endregion
        
    }
}