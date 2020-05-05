
using System;

namespace ReadyGamerOne.Rougelike.Person
{
    /// <summary>
    /// 可以制造和承受伤害
    /// </summary>
    /// <typeparam name="PersonType"></typeparam>
    public interface ITakeDamageablePerson<PersonType>
        where PersonType:AbstractPerson
    {
        /// <summary>
        /// 触发事件会对damage进行四舍五入
        /// </summary>
        event Action<AbstractPerson, int> onCauseDamage;
        event Action<AbstractPerson, int> onTakeDamage;
        

        /// <summary>
        /// 承受这么多伤害
        /// </summary>
        /// <param name="takeDamageFrom">谁打我的</param>
        /// <param name="damage">外界带来的伤害</param>
        /// <returns>实际造成的伤害</returns>
        float OnTakeDamage(PersonType takeDamageFrom,float damage);
        float OnCauseDamage(PersonType causeDamageTo,float damage);

    }
}