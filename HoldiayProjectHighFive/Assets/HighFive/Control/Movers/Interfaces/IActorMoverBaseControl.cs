using ReadyGamerOne.Rougelike.Mover;
using UnityEngine;

namespace HighFive.Control.Movers.Interfaces
{
    /// <summary>
    /// 方便控制Actor的接口
    /// </summary>
    public interface IActorBaseControl:IBaseControl
    {
        /// <summary>
        /// 控制能否移动的总开关
        /// </summary>
        /// <param name="enableMove"></param>
        void SetMovable(bool enableMove);
        /// <summary>
        /// 获取角色面朝方向
        /// </summary>
        int FaceDir { get; set; }
        /// <summary>
        /// 角色是否站在地上
        /// </summary>
        bool IsGrounded { get; }
        
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
        /// 单独方向速度控制
        /// </summary>
        float VelocityX { get; set; }
        float VelocityY { get; set; }
        /// <summary>
        /// 根据传来打击的向量（带有大小）和Multiplier瞬时改变Mover的速度
        /// </summary>
        /// <param name="hitDir"></param>
        /// <param name="multiplier"></param>
        void ChangeVelBasedOnHitDir(Vector2 hitDir, float multiplier = 1);
    }
}
