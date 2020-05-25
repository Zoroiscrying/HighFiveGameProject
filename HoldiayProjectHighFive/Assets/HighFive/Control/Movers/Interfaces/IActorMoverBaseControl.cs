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
        /// 是否忽略主观移动，默认不忽略（为false)
        /// 不接收表示ai和控制不起作用，但还是可以被击退，还受重力作用
        /// </summary>
        bool IgnoreMoverInput { get; set; }
        
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
        
        /// <summary>
        /// 反转ActorMover的输入
        /// </summary>
        void ReverseMovementInputX();
        void ReverseMovementInputY();

        /// <summary>
        /// 停止ActorMover的输入
        /// </summary>
        void StopHorizontallyInput();
        void StopVerticallyInput();
        void StopMoverInput();
        
    }
}
