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

        protected override void InitSensor(AbstractSensor sensor)
        {
            if (sensor is HorizontalVisualSensor visualSensor)
            {
                visualSensor.viewDistance = viewDistance.Value;
            }        
        }
    }
}