using System;
using HighFive.Model.Person;
using ReadyGamerOne.Rougelike.Mover;
using ReadyGamerOne.Rougelike.Person;
using UnityEngine;

namespace HighFive.Script
{
    public class TouchDamage:MonoBehaviour
    {
        public float damageScale=1.0f;
        private void Start()
        {
            var mover = GetComponent<IMover2D>();
            mover.eventOnTriggerEnter += OnMoverTriggerOrColliderEnter;
//            mover.eventOnColliderEnter += OnMoverTriggerOrColliderEnter;
        }

        private void OnMoverTriggerOrColliderEnter(GameObject obj)
        {
//            Debug.Log($"??+{obj.name}");
            var player = obj.gameObject.GetPersonInfo() as IHighFivePerson;
            if (player is IHighFiveCharacter)
            {
//                Debug.Log("Player碰我");
                gameObject.GetPersonInfo().TryAttack(player,damageScale);
            }
        }
    }
}