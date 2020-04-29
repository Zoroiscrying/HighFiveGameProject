using System;
using ReadyGamerOne.Rougelike.Mover;
using UnityEngine;

namespace HighFive.Control.Movers
{
    /// <summary>
    /// 移动器基类
    /// </summary>
    public class BaseMover:AbstractMover
    {
        #region IMover2D

        /// <summary>
        /// 这都是属性
        /// 如果你希望在Inspector看到可以这样，以Velocity为例
        /// 建议所有属性都是virtual，毕竟子类大概率要重写
        /// </summary>
        [SerializeField]private Vector2 velocity;
        public override Vector2 Velocity
        {
            get => velocity;
            set => velocity = value;
        }

        /// <summary>
        /// 可以加默认值
        /// </summary>
        public override float GravityScale { get; set; } = 1;
        
        public override LayerMask ColliderLayers { get; set; }
        public override LayerMask TriggerLayers { get; set; }
        
        public event Action<GameObject, TouchDir> eventOnColliderEnter;
        public event Action<GameObject, TouchDir> eventOnTriggerEnter;        

        #endregion
    }
}