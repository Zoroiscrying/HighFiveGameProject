using System;
using ReadyGamerOne.Rougelike.Mover;
using ReadyGamerOne.Utility;
using UnityEngine;

namespace Game.Scripts
{
    public class TempBulletMover:MonoBehaviour,IMover2D
    {
        public Vector3 Position
        {
            get => Rig.position;
            set => Rig.position = value;
        }

        public Vector2 Velocity
        {
            get => Rig.velocity;
            set => Rig.velocity = value;
        }
        
        #region GravityScale

        private Rigidbody2D rig;
        private Rigidbody2D Rig
        {
            get
            {
                if (!rig)
                    rig = GetComponent<Rigidbody2D>();
                return rig;
            }
        }
        public virtual float GravityScale
        {
            get => Rig.gravityScale;
            set => Rig.gravityScale = value;
        }        

        #endregion


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

        public event Action<GameObject> eventOnColliderEnter;
        public event Action<GameObject> eventOnColliderStay;
        public event Action<GameObject> eventOnColliderExit;
        public event Action<GameObject> eventOnTriggerEnter;
        public event Action<GameObject> eventOnTriggerStay;
        public event Action<GameObject> eventOnTriggerExit;

        private void Update()
        {
            transform.position += Time.deltaTime * new Vector3(Velocity.x, Velocity.y);
        }

        protected virtual void OnCollisionEnter2D(Collision2D collision2D)
        {
            eventOnColliderEnter?.Invoke(collision2D.gameObject);
        }


        protected virtual void OnTriggerEnter2D(Collider2D other)
        {
            if (1 == triggerLayers.value.GetNumAtBinary(other.gameObject.layer))
            {
                eventOnTriggerEnter?.Invoke(other.gameObject);
            }
        }
    }
}