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
        [SerializeField]protected float _relativeFlyHeight = 0; 
        
        /// <summary>
        /// 遇到障碍物如何调正自身方向
        /// 在碰到障碍物需要调整方向的时候，调用此函数
        /// 基类只是简单取反，子类可以重写成更真实的方向判断
        /// </summary>
        /// <param name="hit">为null表示碰到edge，否则表示碰到障碍物</param>
        protected virtual void ModifyVelocityOnCollision(RaycastHit2D? hit)
        {
            if (hit == null)
            {// 边缘检测到了
                if(FlyHeight>MaxFlyHeight && this.Velocity.y>0)
                    this.ReverseMovementInputY();
                else if(FlyHeight<MinFlyHeight && this.Velocity.y<0)
                    this.ReverseMovementInputY();
                else
                {
                    Debug.Log("?????");
                }
            }
            else
            {// 碰到障碍物了
                this.ReverseMovementInputX();
                this.ReverseMovementInputY();
            }
        }

        
        /// <summary>
        /// 此函数处理如何“随机飞行“的逻辑
        /// 现阶段可以简单点，后期方便改成更真实的随即飞行
        /// ”随即飞行“状态下，每帧调用此函数
        /// </summary>
        protected virtual void OnRandomFly()
        {
            _relativeFlyHeight = CalculateRelativeHeight();
            HandleEdgeCondition();
        }


        [ContextMenu("StartFly")]
        private void StartFly()=>StartRandomFly(new Vector2(1,1));
        
        
        #region IFlyActorControl

        [SerializeField] protected bool _isRandomFlying = false;
        public bool IsRandomFlying => _isRandomFlying;
        public float FlyHeight => _relativeFlyHeight;

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
            _isRandomFlying = true;
            this.moverInput = startDir;
        }

        public void StopRandomFly()
        {
            _isRandomFlying = false;
        }

        /// <summary>
        /// 计算相对高度
        /// </summary>
        public float CalculateRelativeHeight()
        {
            //do Ray Cast and get the ground position
            var height = float.MaxValue;

            var hit = Physics2D.Raycast(this.Position, Vector2.down, 1000000f, this.ColliderLayers);
           //设定最远的飞行距离为1000000f，这里可以再考虑一下
           if (hit && hit.distance < 1000000f)
           {
               height = this.Position.y - hit.point.y;
//               Debug.Log($"Position:{Position}, hit.position:{hit.point},relativeHeight:{_relativeFlyHeight}");
           }
           else
           {
//               Debug.Log("???");
           }

           return height;
        }

        /// <summary>
        /// 处理边缘检测
        /// </summary>
        public void HandleEdgeCondition()
        {
            
            //只在大于零时做检测
            if (_minFlyHeight>=0)
            {
                if (this.FlyHeight < _minFlyHeight)
                {
                    //do something.
                    Debug.Log("ToShort");
                    ModifyVelocityOnCollision(null);
                }
            }
            //只在大于零时做检测
            if (_maxFlyHeight>=0)
            {
                if (FlyHeight > maxJumpHeight)
                {
                    //do something.
                    Debug.Log($"ToHigh");
                    ModifyVelocityOnCollision(null);
                }
            }
        }

        #endregion

        #region Monobehavior

        protected override void Awake()
        {
            base.Awake();
            this.eventOnColliderEnter += hit => ModifyVelocityOnCollision(hit);
        }

        protected override void Update()
        {
            base.Update();
            if (_isRandomFlying)
            {
                OnRandomFly();
            }
        }

        #endregion
        
    }
}