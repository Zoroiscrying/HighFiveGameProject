using System;
using HighFive.Control.Movers.Interfaces;
using ReadyGamerOne.Rougelike.Mover;
using ReadyGamerOne.Utility;
using UnityEngine;

namespace Game.Scripts
{
    public class TempMover:MonoBehaviour,IBaseControl
    {
        #region Rigidbody

        private Rigidbody2D rig;
        protected Rigidbody2D Rig
        {
            get
            {
                if (!rig)
                    rig = GetComponent<Rigidbody2D>();
                return rig;
            }
        }        

        #endregion

        public virtual int FaceDir
        {
            get { return transform.localScale.x > 0 ? 1 : -1; }
            set
            {
                var scale = transform.localScale;
                var dir = value > 0 ? 1 : -1;
                transform.localScale = new Vector3(
                    dir * Mathf.Abs(scale.x), scale.y, scale.z);
            }
        }
        protected Vector2 RealOffset =>
            new Vector2(
                FaceDir * centerOffset.x, centerOffset.y);

        public Vector3 Position
        {
            get => Rig.position + RealOffset;
            set
            {
                var offset = RealOffset;
                Rig.position = value-new Vector3(offset.x,offset.y);
            } 
        }
        public Vector2 Velocity
        {
            get => Rig.velocity;
            set => Rig.velocity = value;
        }

        public float Gravity { get=>Physics2D.gravity.y; set=>Physics2D.gravity=new Vector3(0,value); }

        public float GravityScale
        {
            get => Rig.gravityScale;
            set => Rig.gravityScale = value;
        }

        [SerializeField] protected Vector2 centerOffset;
        [SerializeField] protected LayerMask colliderLayers;
        public LayerMask ColliderLayers
        {
            get => colliderLayers;
            set => colliderLayers = value;
        }
        [SerializeField] protected LayerMask triggerLayers;
        public LayerMask TriggerLayers
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
        

        protected virtual void OnCollisionEnter2D(Collision2D collision2D)
        {
            eventOnColliderEnter?.Invoke(collision2D.gameObject);
        }

        private void OnCollisionExit2D(Collision2D other)
        {
            eventOnColliderExit?.Invoke(other.gameObject);
        }

        private void OnCollisionStay2D(Collision2D other)
        {
            eventOnColliderStay?.Invoke(other.gameObject);
        }


        private void OnTriggerStay2D(Collider2D other)
        {
            if (1 == triggerLayers.value.GetNumAtBinary(other.gameObject.layer))
            {
                eventOnTriggerStay?.Invoke(other.gameObject);
            }
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            if (1 == triggerLayers.value.GetNumAtBinary(other.gameObject.layer))
            {
                eventOnTriggerExit?.Invoke(other.gameObject);
            }
        }


        protected virtual void OnTriggerEnter2D(Collider2D other)
        {
//            Debug.Log($"发现：{other.name}");
            if (1 == colliderLayers.value.GetNumAtBinary(other.gameObject.layer))
            {
                eventOnColliderEnter?.Invoke(other.gameObject);
            }
            else if (1 == triggerLayers.value.GetNumAtBinary(other.gameObject.layer))
            {
                eventOnTriggerEnter?.Invoke(other.gameObject);
            }
        }

        private void OnDrawGizmos()
        
        {
            Gizmos.color=new Color(0.95f, 1f, 0.44f);
            Gizmos.DrawWireSphere(Position, 0.5f);
        }
    }
}