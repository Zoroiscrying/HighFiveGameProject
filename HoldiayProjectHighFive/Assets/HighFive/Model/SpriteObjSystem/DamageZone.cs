using System;
using System.Collections.Generic;
using HighFive.Model.Person;
using ReadyGamerOne.Rougelike.Mover;
using ReadyGamerOne.Rougelike.Person;
using UnityEngine;
using UnityEngine.Assertions;

namespace HighFive.Model.SpriteObjSystem
{
    /// <summary>
    /// 有伤害的区域，次脚本会对进入范围的EnemyLayers目标造成一次伤害
    /// </summary>
    public class DamageZone:MonoBehaviour
    {
        private bool _init = false;
        public float damageScale = 1;
        protected IHighFivePerson selfPerson;
        protected IMover2D mover;
        protected bool enableTrigger = true;

        protected virtual Action OnWork => null;
        
        public LayerMask EnemyLayers
        {
            get => mover.TriggerLayers;
            set => mover.TriggerLayers = value;
        }

        public List<Collider2D> caches = new List<Collider2D>();

        protected virtual void Awake()
        {
            
        }
        
        public virtual void Init(IHighFivePerson self)
        {
            Assert.IsNotNull(self);
            selfPerson = self;
            mover = GetComponent<IMover2D>();
            mover.eventOnTriggerEnter += OnEnemyEnter;
            _init = true;
        }

        private void Update()
        {
            if (!_init)
                return;
            OnWork?.Invoke();
        }
        
        protected virtual void OnEnemyEnter(GameObject enemy, TouchDir touchDir)
        {
            if (!enableTrigger)
            {
                Debug.Log("?????");
                return;
            }
            
            Debug.Log(this.GetType());
            if (!selfPerson.gameObject.TryAttack(enemy, damageScale))
            {
                Debug.LogWarning($"无效攻击【{selfPerson?.CharacterName}=>{enemy.name}】");
            }
        }
    }
}