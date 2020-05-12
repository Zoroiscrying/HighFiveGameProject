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
        /// <summary>
        /// 遇到障碍物如何调正自身方向
        /// 在碰到障碍物需要调整方向的时候，调用此函数
        /// 基类只是简单取反，子类可以重写成更真实的方向判断
        /// </summary>
        /// <param name="other"></param>
        protected virtual void ModifyVelocityOnCollision(GameObject other)
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
            throw new NotImplementedException();
        }

        #region IFlyActorControl

        [SerializeField]
        private float _maxFlyHeight;
        public float MaxFlyHeight
        {
            get => _maxFlyHeight;
            set => _maxFlyHeight = value;
        }
        [SerializeField] private float _minFlyHeight;
        public float MinFlyHeight
        {
            get => _minFlyHeight;
            set => _minFlyHeight = value;
        }

        public void StartRandomFly(Vector2 startDir)
        {
            throw new System.NotImplementedException();
        }

        public void StopRandomFly()
        {
            throw new System.NotImplementedException();
        }
        #endregion
        
    }
}