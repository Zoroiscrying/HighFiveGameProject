using System;
using HighFive.Global;
using HighFive.Model.Person;
using ReadyGamerOne.Rougelike.Person;
using ReadyGamerOne.Utility;
using UnityEngine;

namespace HighFive.AI
{
    public class HorizontalVisualSensor:AbstractSensor
    {
        [HideInInspector]public float viewDistance;
        
        public override IHighFivePerson Detect()
        {
            var hit = Physics2D.Raycast(transform.position, SelfPerson.Dir*Vector2.right, viewDistance, detectLayers|terrainLayers);
            if (!hit) 
                return null;
            if (1 != detectLayers.value.GetNumAtBinary(hit.collider.gameObject.layer)) 
                return null; 
            if (hit.collider.gameObject.GetPersonInfo() is IHighFivePerson person && person.IsAlive)
            {
                return person;
            }

            return null;
        }

        protected override void OnDrawGizmos()
        {
            if (SelfPerson == null)
                return;
            base.OnDrawGizmos();
            Gizmos.color=Color.cyan;
            var position = transform.position;
            Gizmos.DrawLine(position,position+viewDistance*SelfPerson.Dir*Vector3.right);
        }
    }
}