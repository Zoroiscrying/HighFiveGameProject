using System;
using HighFive.Model.Person;
using HighFive.Model.SpriteObjSystem;
using ReadyGamerOne.Rougelike.Mover;
using ReadyGamerOne.Rougelike.Person;
using ReadyGamerOne.Script;
using UnityEngine;
using UnityEngine.Assertions;

namespace HighFive.Model
{
    /// <summary>
    /// 子弹类
    /// 需求：
    ///    子弹：
    ///         1、造成伤害
    ///             input:damageScale,HighFivePerson
    ///         2、特效
    ///             击中音效、爆炸震屏、粒子效果
    ///         3、打到地面回调，打到敌人回调
    ///         4、Input:
    ///              enemyLayer,terrainLayer
    ///         5、单体伤害/AOE伤害，半径多少
    ///         6、子弹如何飞行
    /// 
    ///         
    ///         
    /// 
    ///     直线子弹：
    ///         input：dir,speed,shotThrough
    ///         思路：无视重力，设置初始速度后，匀速直线飞行
    ///     抛物线子弹：
    ///         input:initialSpeed
    ///         思路：考虑重力，速度会受重力影响，抛物线运动
    ///     追踪子弹：
    ///         input:inTarget,speed
    ///         思路：
    ///     弹道子弹：
    ///         input：具体看情况
    ///         思路：使用数学公式计算其每一时刻速度
    /// </summary>
    
    
    
    /// <summary>
    /// 子弹基类，子弹会额外检测和地面的碰撞并做一些操作【销毁子弹】
    /// <summary>
    public abstract class AbstractBullet:DamageZone
    {
        public float maxLife = 2.0f;
        public float? gravityScale = null;
        
        public LayerMask TerrainLayers
        {
            get => mover.ColliderLayers;
            set => mover.ColliderLayers = value;
        }

        /// <summary>
        /// 初始化子弹
        /// </summary>
        public override void Init(IHighFivePerson self)
        {
            base.Init(self);
            mover.eventOnColliderEnter += OnTerrainEnter;
            if (null != gravityScale)
                mover.GravityScale = gravityScale.Value;
            MainLoop.Instance.ExecuteLater(DestorySelf, maxLife);
        }
        
        protected virtual void OnTerrainEnter(GameObject terrain, ReadyGamerOne.Rougelike.Mover.TouchDir touchDir)
        {
//            Debug.Log($"hit terrain [{terrain.name}], destory self");
            DestorySelf();
        }
        

        protected void DestorySelf()
        {
            if (this)
                Destroy(this.gameObject);
        }
    }
}