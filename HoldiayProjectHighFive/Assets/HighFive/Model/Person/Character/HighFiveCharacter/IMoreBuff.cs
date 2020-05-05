namespace HighFive.Model.Person
{
    public interface IMoreBuff
    {
        /// <summary>
        /// 脱战回血
        /// </summary>
        float RecoverWhenAfterFight { get; set; }
        /// <summary>
        /// 速度倍率，加速/减速
        /// </summary>
        float SpeedScale { get; set; }
        /// <summary>
        /// 击杀回血
        /// </summary>
        float RecoverWhenKill { get; set; }
        /// <summary>
        /// 攻击回血
        /// </summary>
        float RecoverWhenAttack { get; set; }
    }
}