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

        public List<GameObject> caches = new List<GameObject>();
        
        
        public virtual void Init(IHighFivePerson self)
        {
            Assert.IsNotNull(self);
            selfPerson = self;
            mover = GetComponent<IMover2D>();
            mover.eventOnTriggerEnter += OnEnemyEnter;
            mover.eventOnTriggerExit += OnEnemyExit;
            mover.eventOnTriggerStay += OnEnemyStay;
            _init = true;
        }

        protected virtual void OnEnemyStay(GameObject obj)
        {
            if (enableTrigger)
            {
                if (caches.Contains(obj))
                {
                    OnEnemyEnter(obj);
                    caches.Remove(obj);
                }
            }
        }

        protected virtual void OnEnemyExit(GameObject obj)
        {
            if (caches.Contains(obj))
            {
                caches.Remove(obj);
            }
        }

        private void Update()
        {
            if (!_init)
                return;
            OnWork?.Invoke();
        }
        
        protected virtual void OnEnemyEnter(GameObject enemy)
        {
            if (!enableTrigger)
            {
                caches.Add(enemy);
                return;
            }
            
            if (!selfPerson.gameObject.TryAttack(enemy, damageScale))
            {
                Debug.LogWarning($"无效攻击【{selfPerson?.CharacterName}=>{enemy.name}】");
            }
        }
    }
}