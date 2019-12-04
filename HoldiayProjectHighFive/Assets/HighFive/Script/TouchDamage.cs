using HighFive.Control.PersonSystem.Persons;
using UnityEngine;

namespace HighFive.Script
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
                player.PlayAcceptEffects(AbstractPerson.GetInstance(gameObject));
            }
        }
    }
}