using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using HighFive.Global;
using HighFive.Model.Person;
using ReadyGamerOne.Rougelike.Person;
using UnityEngine;

namespace HighFive.AI.Conditions
{
    public class CanHearPlayer:Detector
    {
        public SharedFloat hearRadius;


        protected override void InitSensor(AbstractSensor sensor)
        {
            if (sensor is HearingSensor hearingSensor)
            {
                hearingSensor.hearingRadius = hearRadius.Value;
            }
        }
    }
}