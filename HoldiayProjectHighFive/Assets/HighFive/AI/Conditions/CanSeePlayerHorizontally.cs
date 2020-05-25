using System;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using HighFive.Model.Person;
using ReadyGamerOne.Rougelike.Person;
using UnityEngine;

namespace HighFive.AI.Conditions
{
    
    [TaskDescription("是否在水平方向看到玩家")]
    public class CanSeePlayerHorizontally:Detector
    {
        [BehaviorDesigner.Runtime.Tasks.Tooltip("视觉距离")]
        public SharedFloat viewDistance;

        protected override bool InitSensor(AbstractSensor sensor)
        {
            if (sensor is VisualSensor visualSensor)
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