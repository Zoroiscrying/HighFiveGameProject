using HighFive.Model.Person;
using ReadyGamerOne.Rougelike.Mover;
using ReadyGamerOne.Rougelike.Person;
using UnityEngine;
using UnityEngine.Assertions;

namespace HighFive.AI
{
    public abstract class AbstractSensor:MonoBehaviour
    {
        public Vector3 centerOffset;
        public Vector3 CenterPosition => centerOffset + SelfPerson.position;
        
        [HideInInspector]public LayerMask detectLayers;
        [HideInInspector]public LayerMask terrainLayers;

        protected IHighFivePerson SelfPerson { get; private set; }
//        protected IMover2D ActorMover { get; private set; }

        public abstract IHighFivePerson Detect();

        protected virtual void Start()
        {
            SelfPerson=gameObject.GetPersonInfo() as IHighFivePerson;
            Assert.IsNotNull(SelfPerson);
//            ActorMover = gameObject.GetComponent<IMover2D>();
//            Assert.IsNotNull(ActorMover);
        }

        protected virtual void Update()
        {
        }

        protected virtual void OnDrawGizmos()
        {
            if (SelfPerson.IsAlive)
            {
                Gizmos.DrawWireSphere(CenterPosition, 0.1f);
            }
        }
    }
}