using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HighFive.StateMachine
{
    public class StateMachineRunner : MonoBehaviour
    {
        private List<IStateMachine> stateMachineList = new List<IStateMachine>();

        /// <summary>
        /// Creates a stateMachine token object which is used to managed to the state of a monobehaviour. 
        /// </summary>
        /// <typeparam CharacterName="T">An Enum listing different state transitions</typeparam>
        /// <param CharacterName="component">The component whose state will be managed</param>
        /// <returns></returns>
        public StateMachine<T> Initialize<T>(MonoBehaviour component) where T : struct, IConvertible, IComparable
        {
            var fsm = new StateMachine<T>(this, component);

            stateMachineList.Add(fsm);

            return fsm;
        }

        /// <summary>
        /// Creates a stateMachine token object which is used to managed to the state of a monobehaviour. Will automatically transition the startState
        /// </summary>
        /// <typeparam CharacterName="T">An Enum listing different state transitions</typeparam>
        /// <param CharacterName="component">The component whose state will be managed</param>
        /// <param CharacterName="startState">The default start state</param>
        /// <returns></returns>
        public StateMachine<T> Initialize<T>(MonoBehaviour component, T startState) where T : struct, IConvertible, IComparable
        {
            var fsm = Initialize<T>(component);

            fsm.ChangeState(startState);

            return fsm;
        }

        void FixedUpdate()
        {
            for (int i = 0; i < stateMachineList.Count; i++)
            {
                var fsm = stateMachineList[i];
                if (!fsm.IsInTransition && fsm.Component.enabled) fsm.CurrentStateMap.FixedUpdate();
            }
        }

        void Update()
        {
            for (int i = 0; i < stateMachineList.Count; i++)
            {
                var fsm = stateMachineList[i];
                if (!fsm.IsInTransition && fsm.Component.enabled)
                {
                    fsm.CurrentStateMap.Update();
                }
            }
        }

        void LateUpdate()
        {
            for (int i = 0; i < stateMachineList.Count; i++)
            {
                var fsm = stateMachineList[i];
                if (!fsm.IsInTransition && fsm.Component.enabled)
                {
                    fsm.CurrentStateMap.LateUpdate();
                }
            }
        }

        //void OnCollisionEnter(Collision collision)
        //{
        //	if(currentState != null && !IsInTransition)
        //	{
        //		currentState.OnCollisionEnter(collision);
        //	}
        //}

        public static void DoNothing()
        {
        }

        public static void DoNothingCollider(Collider other)
        {
        }

        public static void DoNothingCollision(Collision other)
        {
        }

        public static IEnumerator DoNothingCoroutine()
        {
            yield break;
        }
    }
}
