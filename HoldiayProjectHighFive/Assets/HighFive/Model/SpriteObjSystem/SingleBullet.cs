using System;
using HighFive.Model.Person;
using ReadyGamerOne.Rougelike.Mover;
using UnityEngine;

namespace HighFive.Model.SpriteObjSystem
{
    public class SingleBullet:AbstractBullet
    {
        public bool shotThrough;

        public Vector2 initialSpeed;

        private bool shoted = false;

        public override void Init(IHighFivePerson self)
        {
            base.Init(self);
            mover.Velocity = initialSpeed;
        }

        protected override void OnEnemyEnter(GameObject enemy, TouchDir dir)
        {
            base.OnEnemyEnter(enemy, dir);
            //如果不射穿，那么击中后就要销毁自己
            if(!shotThrough)
                DestorySelf();
        }
    }
}