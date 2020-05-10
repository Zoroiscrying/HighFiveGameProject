using System;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using HighFive.Model.Person;
using ReadyGamerOne.Rougelike.Person;
using UnityEngine;

namespace HighFive.AI.Conditions
{
    public class CanSeePlayer:Detector
    {
        public SharedFloat viewDistance;

        protected override bool InitSensor(AbstractSensor sensor)
        {
            
            if (sensor is HorizontalVisualSensor visualSensor)
            {
                visualSensor.viewDistance = viewDistance.Value;
                return true;
            }

            return false;
        }


//        protected override bool OnFindTarget(IHighFivePerson target)
//        {
////            Debug.Log($"{selfPerson.CharacterName} 看到 {target.CharacterName}");
//            return base.OnFindTarget(target);
//        }
    }
}