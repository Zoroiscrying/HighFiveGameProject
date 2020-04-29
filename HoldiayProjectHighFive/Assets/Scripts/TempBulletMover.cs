using System;
using ReadyGamerOne.Rougelike.Mover;
using UnityEngine;

namespace Game.Scripts
{
    public class TempBulletMover:MonoBehaviour,IMover2D
    {
        public virtual Vector2 Velocity { get; set; }=Vector2.zero;
        [SerializeField] private float gravityScale = 1.0f;
        public virtual float GravityScale
        {
            get => gravityScale;
            set => gravityScale = value;
        }

        [SerializeField] private LayerMask colliderLayers;
        public virtual LayerMask ColliderLayers
        {
            get => colliderLayers;
            set => colliderLayers = value;
        }        
        [SerializeField] private LayerMask triggerLayers;
        public virtual LayerMask TriggerLayers
        {
            get => triggerLayers; 
            set=>triggerLayers=value; 
        }

        public event Action<GameObject, ReadyGamerOne.Rougelike.Mover.TouchDir> eventOnColliderEnter;
        public event Action<GameObject, ReadyGamerOne.Rougelike.Mover.TouchDir> eventOnTriggerEnter;


        protected virtual void OnCollisionEnter2D(Collision2D collision2D)
        {
            eventOnColliderEnter?.Invoke(collision2D.gameObject,ReadyGamerOne.Rougelike.Mover.TouchDir.Top);
        }

    }
}