using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using HighFive.Global;
using HighFive.Model.Person;
using ReadyGamerOne.Rougelike.Person;
using UnityEngine;

namespace HighFive.AI.Conditions
{
    [TaskDescription("是否听见玩家，可穿墙")]
    public class CanHearPlayer:Detector
    {
        [BehaviorDesigner.Runtime.Tasks.Tooltip("听觉半径")]
        public SharedFloat hearRadius;

        protected override bool InitSensor(AbstractSensor sensor)
        {
            if (sensor is HearingSensor hearingSensor)
            {
                hearingSensor.hearingRadius = hearRadius.Value;
                return true;
            }

            return false;
        }
    }
}