using System;
using HighFive.Global;
using HighFive.Model.Person;
using ReadyGamerOne.Rougelike.Person;
using ReadyGamerOne.Utility;
using UnityEngine;

namespace HighFive.AI
{
    /// <summary>
    /// 水平视觉探测器
    /// </summary>
    public class HorizontalVisualSensor:VisualSensor
    {
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
    }
}