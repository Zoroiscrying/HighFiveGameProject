#define DEBUG_CC2D_RAYS
using UnityEngine;
using System;
using System.Collections.Generic;


namespace zoroiscrying
{
    /// <summary>
    /// 继承自RaycastController，负责碰撞的检测和实际的移动功能。
    /// 注意，这个Controller所负责的是Trigger的进入、持续和退出事件，所以要设置Collider为Trigger
    /// 本脚本还负责
    /// </summary>
    public class CharacterController2D : RayCastController2D
    {
        #region internal types

        [HideInInspector] public Vector2 playerInput;

        /// <summary>
        /// 角色的碰撞状态
        /// </summary>
        public class CharacterCollisionState2D 
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

        #endregion

        #region events, properties and fields

        public event Action<RaycastHit2D> onControllerCollidedEvent; //如果碰撞会调用这个event
        public event Action<Collider2D> onTriggerEnterEvent; //如果TriggerEnter会调用这个event
        public event Action<Collider2D> onTriggerStayEvent; //如果TriggerStay会调用这个event
        public event Action<Collider2D> onTriggerExitEvent; //同理


        /// <summary>
        /// when true, one way platforms will be ignored when moving vertically for a single frame
        /// </summary>
        public bool ignoreOneWayPlatformsThisFrame; //这一帧忽略OneWayPlatform层，可以穿过

        /// <summary>
        /// mask with all layers that the player should interact with
        /// </summary>
        public LayerMask platformMask = 0;

        /// <summary>
        /// mask with all layers that trigger events should fire when intersected
        /// </summary>
        public LayerMask triggerMask = 0;

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

        [HideInInspector] [NonSerialized] public new Transform transform;
        [HideInInspector] [NonSerialized] public Rigidbody2D rigidBody2D;
        [SerializeField] public CharacterCollisionState2D collisionState = new CharacterCollisionState2D();
        [HideInInspector] [NonSerialized] public Vector3 velocity;

        public bool isGrounded
        {
            get { return collisionState.below; }
        }

        const float kSkinWidthFloatFudgeFactor = 0.001f;

        #endregion

        RaycastHit2D _raycastHit;

        /// <summary>
        /// stores any raycast hits that occur this frame. we have to store them in case we get a hit moving
        /// horizontally and vertically so that we can send the events after all collision state is set
        /// </summary>
        List<RaycastHit2D> _raycastHitsThisFrame = new List<RaycastHit2D>(2);

        // we use this flag to mark the case where we are travelling up a slope and we modified our delta.y to allow the climb to occur.
        // the reason is so that if we reach the end of the slope we can make an adjustment to stay grounded

        #region Monobehaviour

        public override void Awake()
        {
            base.Awake();
            // add our one-way platforms to our normal platform mask so that we can land on them from above
            platformMask |= oneWayPlatformMask;
            collisionState.faceDir = 1;
            // cache some components
            transform = GetComponent<Transform>();
            rigidBody2D = GetComponent<Rigidbody2D>();

            // we want to set our CC2D to ignore all collision layers except what is in our triggerMask
            for (var i = 0; i < 32; i++)
            {
                // see if our triggerMask contains this layer and if not ignore it
                if (((triggerMask.value << i)) == 0)
                    Physics2D.IgnoreLayerCollision(gameObject.layer, i);
            }
        }


        public void OnTriggerEnter2D(Collider2D col)
        {
            if (onTriggerEnterEvent != null)
                onTriggerEnterEvent(col);
        }


        public void OnTriggerStay2D(Collider2D col)
        {
            if (onTriggerStayEvent != null)
                onTriggerStayEvent(col);
        }


        public void OnTriggerExit2D(Collider2D col)
        {
            if (onTriggerExitEvent != null)
                onTriggerExitEvent(col);
        }

        #endregion


        #region Public

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
            PrimeRaycastOrigins();

            // save off our current grounded state which we will use for wasGroundedLastFrame and becameGroundedThisFrame
            collisionState.wasGroundedLastFrame = collisionState.below;
            // clear our state
            collisionState.Reset();
            _raycastHitsThisFrame.Clear();
            playerInput = input;
            
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
                velocity = deltaMovement / Time.deltaTime;

            //如果移动前不在地面上，而移动后到达了地面，则把如下变量设为true
            if (!collisionState.wasGroundedLastFrame && collisionState.below)
                collisionState.becameGroundedThisFrame = true;

            // 委托事件调用
            if (onControllerCollidedEvent != null)
            {
                for (var i = 0; i < _raycastHitsThisFrame.Count; i++)
                    onControllerCollidedEvent(_raycastHitsThisFrame[i]);
            }

            if (standingOnPlatform && !collisionState.wasGroundedLastFrame)
            {
                collisionState.below = true;
                collisionState.becameGroundedThisFrame = true;
            }

            ignoreOneWayPlatformsThisFrame = false;
        }

        public RaycastHit2D GetFirstCastHitBelow()
        {
            RaycastHit2D hit;
            var rayDistance = 2 * skinWidth;
            var rayDirection = Vector2.down;
            var initialRayOrigin = (_raycastOrigins.bottomRight + _raycastOrigins.bottomLeft) / 2;

            var ray = new Vector2(initialRayOrigin.x, initialRayOrigin.y);
            hit = Physics2D.Raycast(ray, rayDirection * rayDistance, platformMask);

            return hit;
        }
        

        public int IfNearWall(Vector2 deltaMovement)
        {
            //var rayDistance = Mathf.Abs( deltaMovement.x ) + skinWidth;
            var rayDistance = 2 * skinWidth;
            var rayDirectionR = Vector2.right;
            var initialRayOriginR = _raycastOrigins.bottomRight;
            
            var rayDirectionL = Vector2.left;
            var initialRayOriginL = _raycastOrigins.bottomLeft;
            RaycastHit2D hit;
            
            if (Mathf.Abs(deltaMovement.x) < skinWidth)
            {
                rayDistance = 2 * skinWidth;
            }

            for (var i = 2; i < 8; i++)
            {
                var ray = new Vector2(initialRayOriginR.x, initialRayOriginR.y + i * _verticalDistanceBetweenRays);

                DrawRay(ray, rayDirectionR * rayDistance, Color.red);

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
            
            //todo::

            for (var i = 2; i < 8; i++)
            {
                var ray = new Vector2(initialRayOriginL.x, initialRayOriginL.y + i * _verticalDistanceBetweenRays);

                DrawRay(ray, rayDirectionL * rayDistance, Color.red);

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
            
            //todo::

            return 0;
        }

        #endregion


        #region Movement Methods

        /// <summary>
        /// 负责Controller的横向碰撞检测
        /// </summary>
        void CheckHorizontalCollision(ref Vector2 deltaMovement)
        {
            var isGoingRight = collisionState.faceDir;
            var rayDistance = Mathf.Abs(deltaMovement.x) + skinWidth;
            var rayDirection = (isGoingRight == 1) ? Vector2.right : -Vector2.right;
            var initialRayOrigin = (isGoingRight == 1) ? _raycastOrigins.bottomRight : _raycastOrigins.bottomLeft;

            if (Mathf.Abs(deltaMovement.x) < skinWidth)
            {
                rayDistance = 2 * skinWidth;
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
                    Debug.Log("Hit Wall");
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
                        deltaMovement.x = (_raycastHit.distance - skinWidth) * rayDirection.x;
                        rayDistance = _raycastHit.distance;

                        collisionState.left = isGoingRight == -1;
                        collisionState.right = isGoingRight == 1;

                        _raycastHitsThisFrame.Add(_raycastHit);
                        // we add a small fudge factor for the float operations here. if our rayDistance is smaller
                        // than the width + fudge bail out because we have a direct impact
                        if (rayDistance < skinWidth + kSkinWidthFloatFudgeFactor)
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
                Mathf.Abs(deltaMovement.y) + skinWidth; //the ray's original starting point is within the collider
            var rayDirection = isGoingUp == 1 ? Vector2.up : -Vector2.up;
            var initialRayOrigin = isGoingUp == 1 ? _raycastOrigins.topLeft : _raycastOrigins.bottomLeft;

            // apply our horizontal deltaMovement here so that we do our raycast from the actual position we would be in if we had moved
            initialRayOrigin.x += deltaMovement.x;

            if (Mathf.Abs(deltaMovement.y) < skinWidth)
            {
                rayDistance = 2 * skinWidth;
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
                    if (_raycastHit.collider.tag == "Through")
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
                    deltaMovement.y = (_raycastHit.distance - skinWidth) * rayDirection.y;
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

                    _raycastHitsThisFrame.Add(_raycastHit); //stores all the hits this frame.

//                    // this is a hack to deal with the top of slopes. if we walk up a slope and reach the apex we can get in a situation
//                    // where our ray gets a hit that is less then skinWidth causing us to be ungrounded the next frame due to residual velocity.
//                    if (!(isGoingUp == 1) && deltaMovement.y > 0.00001f)
//                        collisionState.climbingSlope = true;

                    // we add a small fudge factor for the float operations here. if our rayDistance is smaller
                    // than the width + fudge bail out because we have a direct impact
                    
                    if (rayDistance < skinWidth + kSkinWidthFloatFudgeFactor)
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
    }
}