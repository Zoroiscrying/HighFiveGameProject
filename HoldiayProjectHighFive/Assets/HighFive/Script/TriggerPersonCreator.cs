using HighFive.Control.Movers;
using HighFive.Model.Person;
using ReadyGamerOne.Rougelike.Mover;
using ReadyGamerOne.Rougelike.Person;
using ReadyGamerOne.Script;
using UnityEngine;

#if UNITY_EDITOR
    using UnityEditor;

#endif

namespace HighFive.Script
{
    [RequireComponent(typeof(BaseMover))]
    [RequireComponent(typeof(ShowCollider2D))]
    public class TriggerPersonCreator:PersonCreator
    {
        
        public float timeDelay = 0f;

        public bool workOnce = true;

        private bool worked = false;

        public override Color DefaultGizmoColor => Color.green;

        protected override void Start()
        {
            base.Start();
            var mover = GetComponent<IMover2D>();
            mover.eventOnTriggerEnter += OnPlayerEnter;
        }
        
        private void OnPlayerEnter(GameObject obj)
        {
            if (obj.GetPersonInfo() is IHighFivePerson)
            {
                if (!worked || !workOnce)
                {
                    MainLoop.Instance.ExecuteLater(Create,timeDelay);
                    worked = true;
                }
            }
        }


        protected override void OnDrawGizmos()
        {
            if (!gameObject.activeSelf || !enabled)
                return;
            base.OnDrawGizmos();
#if UNITY_EDITOR
            Handles.Label(transform.position,$"[PersonTrigger:{name}]",Style);
#endif
        }
    }
}