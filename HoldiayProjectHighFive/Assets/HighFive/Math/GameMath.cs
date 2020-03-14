using HighFive.Model.Person;

namespace HighFive.Math
{
    public static partial class GameMath
    {
        public static int Damage(int damage, float multiplier = 1, float adder = 0)
        {
            return (int)(damage * multiplier + adder);
        }

        public static int Damage(IHighFivePerson attacker, IHighFivePerson receiver)
        {
            return (int) (attacker.Attack * attacker.AttackScaler +
                          attacker.AttackAdder);
        }
    }
}
