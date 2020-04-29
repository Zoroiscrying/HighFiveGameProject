using System;
using ReadyGamerOne.Rougelike.Mover;
using UnityEngine;

namespace HighFive.Control.Movers
{
    /// <summary>
    /// 这个类规定移动器的接口
    /// </summary>
    public abstract class AbstractMover:MonoBehaviour,IMover2D
    {

        #region IMover2D

        public abstract Vector2 Velocity { get; set; }
        public abstract float GravityScale { get; set; }
        public abstract LayerMask ColliderLayers { get; set; }
        public abstract LayerMask TriggerLayers { get; set; }
        
        public event Action<GameObject, TouchDir> eventOnColliderEnter;
        public event Action<GameObject, TouchDir> eventOnTriggerEnter;        

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