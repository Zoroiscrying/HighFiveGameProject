using System;
using ReadyGamerOne.Rougelike.Mover;
using UnityEngine;

namespace HighFive.Control.Movers
{
    /// <summary>
    /// 移动器基类
    /// </summary>
    public class BaseMover:MonoBehaviour,IMover2D
    {
        #region IMover2D

        /// <summary>
        /// 这都是属性
        /// 如果你希望在Inspector看到可以这样，以Velocity为例
        /// 建议所有属性都是virtual，毕竟子类大概率要重写
        /// </summary>
        [SerializeField]private Vector2 velocity;
        public virtual Vector2 Velocity
        {
            get => velocity;
            set => velocity = value;
        }

        /// <summary>
        /// 可以加默认值
        /// </summary>
        public virtual float GravityScale { get; set; } = 1;
        
        public virtual LayerMask ColliderLayers { get; set; }
        public virtual LayerMask TriggerLayers { get; set; }
        public event Action<GameObject, ReadyGamerOne.Rougelike.Mover.TouchDir> eventOnColliderEnter;
        public event Action<GameObject, ReadyGamerOne.Rougelike.Mover.TouchDir> eventOnTriggerEnter;

        #endregion
        
        #region UnityCallBacks_子类如果想用的话一定要override父类的，自己另写的话父类就不会调用了

        protected virtual void Awake(){}

        protected virtual void Start(){}

        protected virtual void Update(){}

        protected virtual void FixedUpdate(){}

        protected virtual void OnTriggerEnter2D(Collider2D collider2D){}

        protected virtual void OnCollisionEnter2D(Collision2D collision2D){}        

        #endregion
    }
}