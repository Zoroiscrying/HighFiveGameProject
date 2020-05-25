using UnityEngine;

namespace HighFive.Control.Movers.Interfaces
{
    public interface IFlyActorControl:IActorBaseControl
    {
        /// <summary>
        /// 是否正在随机飞
        /// </summary>
        bool IsRandomFlying { get; }
        
        /// <summary>
        /// 离地高度
        /// </summary>
        float FlyHeight { get;}
        
        /// <summary>
        /// 最大离地高度，如果小于0，表示无限制
        /// </summary>
        float MaxFlyHeight { get; set; }
        
        /// <summary>
        /// 最小离地距离，如果小于0，表示可以贴地飞行
        /// </summary>
        float MinFlyHeight { get; set; }
        
        /// <summary>
        /// 调用后Mover进入随机飞行状态，直到调用StopRandomFly
        /// </summary>
        /// <param name="startDir"></param>
        void StartRandomFly(Vector2 startDir);
        void StopRandomFly();

        /// <summary>
        /// 初始化FlyMover参考的地面
        /// </summary>
        float CalculateRelativeHeight();
        
        /// <summary>
        /// 检查飞行状态，碰到边缘情况的处理在这里实现
        /// </summary>
        void HandleEdgeCondition();
    }
}