using System;
using System.Collections;
using UnityEngine;

namespace HighFive.StateMachine
{
    public class StateMapping
    {
        public object state;

        public bool hasEnterRoutine;
        public Action EnterCall = StateMachineRunner.DoNothing;
        public Func<IEnumerator> EnterRoutine = StateMachineRunner.DoNothingCoroutine;

        public bool hasExitRoutine;
        public Action ExitCall = StateMachineRunner.DoNothing;
        public Func<IEnumerator> ExitRoutine = StateMachineRunner.DoNothingCoroutine;

        public Action Finally = StateMachineRunner.DoNothing;
        public Action Update = StateMachineRunner.DoNothing;
        public Action LateUpdate = StateMachineRunner.DoNothing;
        public Action FixedUpdate = StateMachineRunner.DoNothing;
        public Action<Collision> OnCollisionEnter = StateMachineRunner.DoNothingCollision;

        public StateMapping(object state)
        {
            this.state = state;
        }

    }
}
