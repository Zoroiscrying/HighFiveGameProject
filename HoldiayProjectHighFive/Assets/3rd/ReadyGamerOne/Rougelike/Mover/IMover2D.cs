using System;
using UnityEngine;

namespace ReadyGamerOne.Rougelike.Mover
{
    public interface IMover2D
    {
        /// <summary>
        /// 设置和获取位置
        /// </summary>
        Vector3 Position { get; set; }
        
        /// <summary>
        /// 获取和设置速度
        /// </summary>
        Vector2 Velocity { get; set; }

        /// <summary>
        /// 重力实际值
        /// </summary>
        float Gravity { get; set; }
        
        /// <summary>
        /// 重力缩放值
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

        event Action<GameObject> eventOnColliderEnter;
        event Action<GameObject> eventOnColliderStay;
        event Action<GameObject> eventOnColliderExit;
        event Action<GameObject> eventOnTriggerEnter;
        event Action<GameObject> eventOnTriggerStay;
        event Action<GameObject> eventOnTriggerExit;
    }
}