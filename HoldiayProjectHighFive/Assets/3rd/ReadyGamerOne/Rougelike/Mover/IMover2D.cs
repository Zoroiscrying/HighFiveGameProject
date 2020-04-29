using System;
using UnityEngine;

namespace ReadyGamerOne.Rougelike.Mover
{
    public enum TouchDir
    {
        Top,
        Bottom,
        Left,
        Right,
    }
   
    public interface IMover2D
    {
        /// <summary>
        /// 获取和设置速度
        /// </summary>
        Vector2 Velocity { get; set; }

        /// <summary>
        /// 重力缩放
        /// </summary>
        float GravityScale { get; set; }
        
        /// <summary>
        /// 会和什么层发生碰撞碰撞
        /// </summary>
        LayerMask ColliderLayers { get; set; }
        
        /// <summary>
        /// 会和那些层触发Trigger但不碰撞
        /// </summary>
        LayerMask TriggerLayers { get; set; }

        event Action<GameObject, TouchDir> eventOnColliderEnter;
        event Action<GameObject, TouchDir> eventOnTriggerEnter;
    }
}