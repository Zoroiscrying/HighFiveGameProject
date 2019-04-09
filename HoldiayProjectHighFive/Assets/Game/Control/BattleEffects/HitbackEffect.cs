﻿using Game.Control.Person;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Game.Control.BattleEffects
{
    /// <summary>
    /// 击退效果
    /// </summary>
    public class HitbackEffect : AbstractBattleEffect
    {
        private Vector2 hit;

        public HitbackEffect(Vector2 hit)
        {
            this.hit = hit;
        }
        public override void Execute(AbstractPerson ap)
        {
            #region 加力
            //            var rig = ap.obj.GetComponent<Rigidbody2D>();
            //            if(rig==null)
            //                Debug.LogError(ap.name+"没有Rigidbody2D");
            //            rig.AddForce(new Vector2(this.dir*this.InstantSpeed,0));
            #endregion

            #region Transform

            if (!ap.IgnoreHitback)
            {
                var trans = ap.obj.transform;
                trans.position += new Vector3(this.hit.x, this.hit.y, 0);
            }

            #endregion

            this.Release(ap);
        }
    }
}
