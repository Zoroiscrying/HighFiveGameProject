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
            if (!SelfPerson.IsAlive)
                return null;
            
            var hit = Physics2D.Raycast(CenterPosition, SelfPerson.Dir*Vector2.right, viewDistance, detectLayers|terrainLayers);
            if (!hit) 
                return null;
//            print($"碰到：{hit.transform.name} _1");
            if (1 != detectLayers.value.GetNumAtBinary(hit.collider.gameObject.layer)) 
                return null; 
//            print($"碰到：{hit.transform.name} _2");
            if (hit.collider.gameObject.GetPersonInfo() is IHighFivePerson person && person.IsAlive)
            {
//                print($"碰到：{hit.transform.name} _3");
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
            var position = CenterPosition;
            Gizmos.DrawLine(position,position+viewDistance*SelfPerson.Dir*Vector3.right);
        }
    }
}