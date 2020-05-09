using UnityEngine;

namespace HighFive.Control.Movers.Interfaces
{
    public interface IActorMoveAIControl:IActorBaseControl
    {
        //Jump
        float JumpStopTime { get; set; }
        bool JumpAllTheTime { get; set; }
        void StartJumping(Vector2 force, float jumpStopTimeS, int jumpCountMaxS, bool loopJump);
        void StopJumping();
        //Patrol
        float PatrolStopTime { get; set; }
        bool PatrolAllTheTime { get; set; }
        void StartPatrolling(Vector2 dir, float patrolStopTimeS, float distance, bool loopMove);
        void StopPatrolling();
        //Movement
    }
}
