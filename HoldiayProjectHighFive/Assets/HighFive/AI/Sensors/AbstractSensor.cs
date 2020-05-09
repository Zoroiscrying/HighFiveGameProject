using HighFive.Model.Person;
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

        public abstract IHighFivePerson Detect();

        protected virtual void Start()
        {
            SelfPerson=gameObject.GetPersonInfo() as IHighFivePerson;
            Assert.IsNotNull(SelfPerson);
        }

        protected virtual void Update()
        {
        }

        protected virtual void OnDrawGizmos()
        {
        }
    }
}