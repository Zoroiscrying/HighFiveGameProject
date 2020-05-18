using HighFive.Model.Person;
using ReadyGamerOne.Rougelike.Person;
using ReadyGamerOne.Utility;
using UnityEngine;

namespace HighFive.AI
{
    public class HearingSensor:AbstractSensor
    {
        [HideInInspector] public float hearingRadius;
        private RaycastHit2D[] hitInfos;
        protected override void Start()
        {
            base.Start();
            hitInfos=new RaycastHit2D[1];
        }

        public override IHighFivePerson Detect()
        {
            var size = Physics2D.CircleCastNonAlloc(CenterPosition, hearingRadius, Vector2.zero, hitInfos, 0, detectLayers);
            if (size == 0)
                return null;
            foreach (var hit in hitInfos)
            {
                if (!hit) 
                    continue;
                if (1 != detectLayers.value.GetNumAtBinary(hit.collider.gameObject.layer)) 
                    continue; 
                if (hit.collider.gameObject.GetPersonInfo() is IHighFivePerson person && person.IsAlive)
                {
//                    Debug.Log($"hearing: {person.CharacterName}");
                    return person;
                }             
            }
            
            return null;
        }


        protected override void OnDrawGizmos()
        {
            if (SelfPerson == null)
                return;
            
            base.OnDrawGizmos();
            Gizmos.color=Color.yellow;
            Gizmos.DrawWireSphere(CenterPosition, hearingRadius);
        }
    }
}