using HighFive.Const;
using HighFive.Control.PersonSystem.Persons;
using HighFive.View;
using ReadyGamerOne.Common;
using UnityEngine;
using UnityEngine.Assertions;

namespace HighFive.Math
{
    public static class GameLogic
    {
        public static void CauseDamageTo(this AbstractPerson attacker, AbstractPerson receiver)
        {
            Assert.IsTrue(attacker != null && receiver != null);
            if (receiver.IsConst)
                return;


            var damage = GameMath.Damage(attacker, receiver);

            //伤害数字
            new NumberTipUI(damage, 0.3f * receiver.Scanler, 24, Color.red, receiver.obj.transform, receiver.Dir, 1);

            CEventCenter.BroadMessage(Message.M_BloodChange(receiver.obj), -damage);
            receiver.OnTakeDamage(damage);
            if (!(receiver is Player))
            {
                CEventCenter.BroadMessage(Message.M_ChangeSmallLevel, damage);
            }

        }
    }
}