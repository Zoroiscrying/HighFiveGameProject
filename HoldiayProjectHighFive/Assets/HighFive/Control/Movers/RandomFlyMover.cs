using System;
using ReadyGamerOne.Utility;
using UnityEngine;
using Random = UnityEngine.Random;

namespace HighFive.Control.Movers
{
    public class RandomFlyMover:FlyActorMover
    {
        protected override void ModifyVelocityOnCollision(RaycastHit2D? hit)
        {
            if (null == hit)
            {
                base.ModifyVelocityOnCollision(null);
                return;
            }

            var info = hit.Value;
            this.MoverInput = info.normal.RotateDegree(Random.Range(-60, 60));
        }
    }
}