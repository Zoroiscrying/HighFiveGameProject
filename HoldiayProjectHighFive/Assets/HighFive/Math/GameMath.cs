using HighFive.Control.PersonSystem.Persons;

namespace HighFive.Math
{
    public static partial class GameMath
    {
        public static int Damage(int damage, float multiplier = 1, float adder = 0)
        {
            return (int)(damage * multiplier + adder);
        }

        public static int Damage(AbstractPerson attacker, AbstractPerson receiver)
        {
            return (int) (attacker.BaseAttack * attacker.attack_scaler +
                          attacker.attack_adder);
        }
    }
}
