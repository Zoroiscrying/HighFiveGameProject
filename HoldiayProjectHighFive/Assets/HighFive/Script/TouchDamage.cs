using HighFive.Model.Person;
using ReadyGamerOne.Rougelike.Person;
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
            var player = other.gameObject.GetPersonInfo() as IHighFivePerson;
            if (player is IHighFiveCharacter)
            {
//                Debug.Log("Player碰我");
                gameObject.GetPersonInfo().TryAttack(player);
            }
        }
    }
}