using System;
using ReadyGamerOne.Utility;
using UnityEngine;
using Random = UnityEngine.Random;

namespace HighFive.Control.Movers
{
    public class RandomFlyMover:FlyActorMover
    {
        protected override void Awake()
        {
            base.Awake();
            var temp = Vector2.right;
            Debug.Log($"temp:{temp}, temp.Rotate(135):{temp.RotateDegree(135)}");
        }

        protected override void ModifyVelocityOnCollision(RaycastHit2D? hit)
        {
            if (null == hit)
            {
                base.ModifyVelocityOnCollision(null);
                return;
            }

            var info = hit.Value;
            this.Velocity = info.normal.RotateDegree(Random.Range(-60, 60)).normalized * Velocity.magnitude;
            throw new Exception($"Normal:{info.normal},Velocity{this.Velocity}");
        }
    }
}