using UnityEngine;

namespace HighFive.Model.Person
{
    public interface IRichDamage
    {
        /// <summary>
        /// 暴击概率
        /// </summary>
        float CritRate { get; set; }
        
        /// <summary>
        /// 暴击倍率
        /// </summary>
        float CritScale { get; set; }
        
        /// <summary>
        /// 是否是无敌状态
        /// </summary>
        bool IsInvincible { get; set; }
        /// <summary>
        /// 击退
        /// </summary>
        Vector2 Repulse { get; }
        /// <summary>
        /// 击退强度
        /// </summary>
        float RepulseScale { get; set; }
        /// <summary>
        /// 无视击退
        /// </summary>
        bool IgnoreRepulse { get; set; }
        /// <summary>
        /// 攻击速度
        /// </summary>
        float AttackSpeed { get; set; }
        /// <summary>
        /// 攻击力加成
        /// </summary>
        float AttackAdder { get; set; }
        /// <summary>
        /// 攻击力倍率
        /// </summary>
        float AttackScale { get; set; }
        /// <summary>
        /// 承受伤害倍率，用于制作“削弱”、“增伤”、“减伤”
        /// </summary>
        float TakeDamageScale { get; set; }
        /// <summary>
        /// 固定伤害格挡
        /// </summary>
        float TakeDamageBlock { get; set; }
        /// <summary>
        /// 闪避概率
        /// </summary>
        float DodgeRate { get; set; }
    }
}