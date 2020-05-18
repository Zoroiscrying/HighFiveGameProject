using System;
using System.Collections.Generic;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using HighFive.Model.Person;
using ReadyGamerOne.Rougelike.Mover;
using ReadyGamerOne.Rougelike.Person;
using UnityEngine;
using UnityEngine.Assertions;

namespace HighFive.AI.Conditions
{
    public abstract class Detector:Conditional
    {
        [BehaviorDesigner.Runtime.Tasks.Tooltip("探测到的敌方单位")]
        public SharedGameObject outDetectedPerson;
        [BehaviorDesigner.Runtime.Tasks.Tooltip("探测的层级")]
        public LayerMask inDetectLayers;
        protected IHighFivePerson selfPerson { get; private set; }
        protected List<AbstractSensor> Sensors { get; private set; }

        public override void OnAwake()
        {
            base.OnAwake();
            selfPerson=gameObject.GetPersonInfo() as IHighFivePerson;
            Assert.IsNotNull(selfPerson);
            Sensors = new List<AbstractSensor>();
            foreach (var sensor in gameObject.GetComponents<AbstractSensor>())
            {
                if(!InitSensor(sensor))
                    continue;
                sensor.detectLayers = inDetectLayers;
                sensor.terrainLayers = selfPerson.ActorMover.ColliderLayers;
                Sensors.Add(sensor);
            }
        }

        protected abstract bool InitSensor(AbstractSensor sensor);

        protected virtual bool OnFindTarget(IHighFivePerson target) => true;
        
        public override TaskStatus OnUpdate()
        {
            if(Sensors.Count==0)
                throw new Exception($"物体【{gameObject.name}】没有感知器");
            
            foreach (var sensor in Sensors)
            {
                var target = sensor.Detect();
                if (target != null && OnFindTarget(target))
                {
                    outDetectedPerson.Value = target.gameObject;
                    return TaskStatus.Success;
                }
            }

            outDetectedPerson.Value = null;
            return TaskStatus.Failure;
        }
    }
}