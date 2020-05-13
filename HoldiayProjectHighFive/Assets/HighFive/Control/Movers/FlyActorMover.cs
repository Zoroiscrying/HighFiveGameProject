using System;
using HighFive.Control.Movers.Interfaces;
using UnityEngine;

namespace HighFive.Control.Movers
{
    /// <summary>
    /// 飞行Mover移动器
    /// </summary>
    public class FlyActorMover:ActorMover,IFlyActorControl
    {
        private bool _randomFly = false;
        private float _relativeGroundPosY = 0; 
        
        /// <summary>
        /// 遇到障碍物如何调正自身方向
        /// 在碰到障碍物需要调整方向的时候，调用此函数
        /// 基类只是简单取反，子类可以重写成更真实的方向判断
        /// </summary>
        /// <param name="collidingObject"></param>
        protected virtual void ModifyVelocityOnCollision(RaycastHit2D collidingObject)
        {
            Velocity = -Velocity;
        }

        
        /// <summary>
        /// 此函数处理如何“随机飞行“的逻辑
        /// 现阶段可以简单点，后期方便改成更真实的随即飞行
        /// ”随即飞行“状态下，每帧调用此函数
        /// </summary>
        protected virtual void OnRandomFly()
        {
            HandleEdgeCondition();
        }

        #region IFlyActorControl
        public float FlyHeight => this.Position.y-_relativeGroundPosY;

        [SerializeField] protected float _maxFlyHeight;

        public float MaxFlyHeight
        {
            get => _maxFlyHeight;
            set => _maxFlyHeight = value;
        }
        [SerializeField] protected float _minFlyHeight;
        public float MinFlyHeight
        {
            get => _minFlyHeight;
            set => _minFlyHeight = value;
        }

        public void StartRandomFly(Vector2 startDir)
        {
            _randomFly = true;
            this.moverInput = startDir;
        }

        public void StopRandomFly()
        {
            _randomFly = false;
        }

        public void InitializeGroundPosition()
        {
            //do Ray Cast and get the ground position
            _relativeGroundPosY = this.Position.y;

           var hit = Physics2D.Raycast(this.Position, Vector2.down, 1000000f, this.ColliderLayers);
           //设定最远的飞行距离为1000000f，这里可以再考虑一下
           if (hit && hit.distance < 1000000f)
           {
               _relativeGroundPosY = this.Position.y - hit.distance;
           }
        }

        public void HandleEdgeCondition()
        {
            //只在大于零时做检测
            if (_minFlyHeight>=0)
            {
                if (this.FlyHeight < _minFlyHeight)
                {
                    //do something.
                }
            }
            //只在大于零时做检测
            if (_maxFlyHeight>=0)
            {
                if (FlyHeight > maxJumpHeight)
                {
                    //do something.
                }
            }
        }

        #endregion

        #region Monobehavior

        protected override void Awake()
        {
            base.Awake();
            InitializeGroundPosition();
            this.eventOnColliderEnter += ModifyVelocityOnCollision;
        }

        protected override void Update()
        {
            base.Update();
            if (_randomFly)
            {
                OnRandomFly();
            }
        }

        #endregion
        
    }
}