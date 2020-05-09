using HighFive.Model.Person;
using ReadyGamerOne.Rougelike.Mover;
using ReadyGamerOne.Rougelike.Person;
using UnityEngine;
using UnityEngine.Assertions;

namespace HighFive.AI
{
    public abstract class AbstractSensor:MonoBehaviour
    {
        [HideInInspector]public LayerMask detectLayers;
        [HideInInspector]public LayerMask terrainLayers;

        protected IHighFivePerson SelfPerson { get; private set; }
//        protected IMover2D SelfMover { get; private set; }

        public abstract IHighFivePerson Detect();

        protected virtual void Start()
        {
            SelfPerson=gameObject.GetPersonInfo() as IHighFivePerson;
            Assert.IsNotNull(SelfPerson);
//            SelfMover = gameObject.GetComponent<IMover2D>();
//            Assert.IsNotNull(SelfMover);
        }

        protected virtual void Update()
        {
        }

        protected virtual void OnDrawGizmos()
        {
        }
    }
}