using UnityEngine;

namespace HighFive.Control.Movers.Interfaces
{
    /// <summary>
    /// 方便控制Actor的接口，测试中...
    /// </summary>
    public interface IActorBaseControl
    {
        /// <summary>
        /// 反转ActorMover的输入
        /// </summary>
        void ReverseMovementInputX();
        void ReverseMovementInputY();
        /// <summary>
        /// 根据目标方向改变ActorMover的Input
        /// </summary>
        /// <param name="target"></param>
        void MoveToward(Vector2 target);
        /// <summary>
        /// 停止ActorMover的输入
        /// </summary>
        void StopHorizontallyInput();
        void StopVerticallyInput();
        void StopMoverInput();
        /// <summary>
        /// 瞬时改变ActorMover的速度
        /// </summary>
        /// <param name="vel"></param>
        void ChangeHorizontalVelocityInstantly(float vel);
        void ChangeVerticalVelocityInstantly(float vel);
        /// <summary>
        /// 根据传来打击的向量（带有大小）和Multiplier瞬时改变Mover的速度
        /// </summary>
        /// <param name="hitDir"></param>
        /// <param name="multiplier"></param>
        void ChangeVelBasedOnHitDir(Vector2 hitDir, float multiplier = 1);
    }
}
