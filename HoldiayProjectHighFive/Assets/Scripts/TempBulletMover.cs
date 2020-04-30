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
        
    }
}