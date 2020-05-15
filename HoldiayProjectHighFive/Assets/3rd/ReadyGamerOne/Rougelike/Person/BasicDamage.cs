namespace ReadyGamerOne.Rougelike.Person
{
    public class BasicDamage
    {
        public float OriginAttack { get; protected set; }
        public float Damage { get; protected set; }
        public float ScaleSkillDamageScale { get; protected set; }

        public BasicDamage()
        {
        }

        public BasicDamage(IPerson person, float skillDamageScale, IPerson another)
        {
            this.ScaleSkillDamageScale = skillDamageScale;
            this.OriginAttack = person.Attack;
            Damage = person.Attack * skillDamageScale;
        }
    }
}