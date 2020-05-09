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
        public SharedGameObject outDetectedPerson;
        public LayerMask inDetectLayers;
        private LayerMask inTerrainLayers;
        protected IHighFivePerson selfPerson { get; private set; }
        protected List<AbstractSensor> Sensors { get; private set; }

        public override void OnAwake()
        {
            base.OnAwake();
            selfPerson=gameObject.GetPersonInfo() as IHighFivePerson;
            Assert.IsNotNull(selfPerson);
            var mover = gameObject.GetComponent<IMover2D>();
            Assert.IsNotNull(mover);
            inTerrainLayers = mover.ColliderLayers;
            Sensors = new List<AbstractSensor>();
            Sensors.AddRange(gameObject.GetComponents<AbstractSensor>());
            foreach (var sensor in Sensors)
            {
                sensor.detectLayers = inDetectLayers;
                sensor.terrainLayers = inTerrainLayers;
                InitSensor(sensor);
            }
        }

        protected abstract void InitSensor(AbstractSensor sensor);


        public override TaskStatus OnUpdate()
        {
            if(Sensors.Count==0)
                throw new Exception($"物体【{gameObject.name}】没有感知器");
            
            foreach (var sensor in Sensors)
            {
                var target = sensor.Detect();
                if (target != null)
                {
                    outDetectedPerson.Value = target.gameObject;
//                    Debug.Log($"发现：{target.CharacterName}");
                    return TaskStatus.Success;
                }
            }

            outDetectedPerson.Value = null;
            return TaskStatus.Failure;
        }
    }
}