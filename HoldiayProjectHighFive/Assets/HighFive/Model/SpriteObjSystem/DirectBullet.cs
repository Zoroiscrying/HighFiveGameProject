using HighFive.Model.Person;
using ReadyGamerOne.Rougelike.Mover;
using UnityEngine;

namespace HighFive.Model.SpriteObjSystem
{
    public class DirectBullet:AbstractBullet
    {
        public bool shotThrough;

        public Vector2 initialSpeed;

        public override void ShotStart(IHighFivePerson self)
        {
            base.ShotStart(self);
            mover.Velocity = initialSpeed;
            mover.GravityScale = 0;
        }

        protected override void OnEnemyEnter(GameObject enemy, TouchDir touchDir)
        {
            base.OnEnemyEnter(enemy, touchDir);
            //如果不射穿，那么击中后就要销毁自己
            if(!shotThrough)
                DestorySelf();
        }
    }
}