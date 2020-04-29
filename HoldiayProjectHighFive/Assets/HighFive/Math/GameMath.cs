namespace HighFive.Math
{
    public static partial class GameMath
    {
        public static int Damage(int damage, float multiplier = 1, float adder = 0)
        {
            return (int)(damage * multiplier + adder);
        }
    }
}
