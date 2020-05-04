namespace HighFive.Control.EffectSystem
{
    public interface IEffector<T>
    where T:class
    {
        /// <summary>
        /// 攻击特效Id列表
        /// </summary>
        EffectInfoAsset AttackEffects { get; }
        
        /// <summary>
        /// 击中特效Id列表
        /// </summary>
        EffectInfoAsset HitEffects { get; }
        
        /// <summary>
        /// 受击特效Id列表
        /// </summary>
        EffectInfoAsset AcceptEffects { get; }
        
        /// <summary>
        /// 特效持有者
        /// </summary>
        T EffectPlayer { get; }

        void PlayAcceptEffects(IEffector<T> ditascher);

        void PlayAttackEffects(EffectInfoAsset effectInfoAsset);
    }
}