using System;
using Game.Control.BattleEffectSystem;
using Game.Control.PersonSystem;
using ReadyGamerOne.Global;
using UnityEngine;

namespace Game.Scripts
{
    public class TouchDamage:MonoBehaviour
    {

        public int damage;
        public Vector2 hitback;


        private void OnTriggerEnter2D(Collider2D other)
        {
//            Debug.Log("有人碰我"+other.transform.name);
            var player = AbstractPerson.GetInstance(other.gameObject);
            if (player is Player)
            {
//                Debug.Log("Player碰我");
                var dir = GlobalVar.G_Player.Pos.x > transform.position.x ? 1 : -1;
                player.TakeBattleEffect(new HitbackEffect(dir*hitback));
                player.TakeBattleEffect(new InstantDamageEffect(damage, transform.position.x > player.Pos.x ? 1 : -1));
            }
        }
    }
}