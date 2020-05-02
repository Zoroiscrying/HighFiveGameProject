using System;
using HighFive.Model.Person;
using UnityEngine;

namespace HighFive.Model.SpriteObjSystem
{
    public class CurvedBullet:AbstractBullet
    {
        public int xDir = 1;
        public int yDir = 1;
        public AnimationCurve aniX;
        public AnimationCurve aniY;

        private float timer;

        protected override Action OnWork => () =>
        {
            this.mover.Velocity =
                new Vector2(
                    xDir * aniX.Evaluate(timer),
                    yDir * aniY.Evaluate(timer));
            timer += Time.deltaTime;
        };
    }
}