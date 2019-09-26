using System;
using Game.Control.BattleEffectSystem;
using Game.Control.PersonSystem;
using UnityEngine;

namespace Game.Scripts
{
    public class TouchDamage:MonoBehaviour
    {

        public int damage;
        public Vector2 hitback;
        private void OnTriggerEnter2D(Collider2D other)
        {
            var player = AbstractPerson.GetInstance(other.gameObject);
            if (player is Player)
            {
                player.TakeBattleEffect(new HitbackEffect(hitback));
                player.TakeBattleEffect(new InstantDamageEffect(damage, transform.position.x > player.Pos.x ? 1 : -1));
            }
        }
    }
}