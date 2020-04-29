using System;
using ReadyGamerOne.Attributes;
using ReadyGamerOne.Rougelike.Mover;
using ReadyGamerOne.Utility;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Game.Scripts
{
    public class TempBulletMover:MonoBehaviour,IMover2D
    {
        [RequireInterface(typeof(IMover2D))]
        public Object mover;
        
        
        public virtual Vector2 Velocity { get; set; }=Vector2.zero;
        [SerializeField] private float gravity = 1.0f;
        public virtual float GravityScale
        {
            get => gravity;
            set => gravity = value;
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

        private void Update()
        {
            transform.position += Time.deltaTime * new Vector3(Velocity.x, Velocity.y);
        }

        protected virtual void OnCollisionEnter2D(Collision2D collision2D)
        {
            eventOnColliderEnter?.Invoke(collision2D.gameObject,ReadyGamerOne.Rougelike.Mover.TouchDir.Top);
        }


        protected virtual void OnTriggerEnter2D(Collider2D other)
        {
            if(1==triggerLayers.value.GetNumAtBinary(other.gameObject.layer))
                eventOnTriggerEnter?.Invoke(other.gameObject,ReadyGamerOne.Rougelike.Mover.TouchDir.Top);
        }
    }
}