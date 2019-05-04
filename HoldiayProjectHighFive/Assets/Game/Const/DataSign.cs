namespace Game.Const
{
    /// <summary>
    /// 保存所有继承 ITxtSerialiable接口的类的标识
    /// 初始化的时候TxtManager类需要注册这些字符串，否则无法使用
    /// </summary>
    public static class DataSign
    {
        public static readonly char txtSplitChar = ',';

        public static readonly string L1Rank = "L1Rank";
        public static readonly string L2Rank = "L2Rank";

        public static readonly string S1Rank = "S1Rank";
        public static readonly string S2Rank = "S2Rank";
        
        public static readonly string skill="skill";
        
        public static readonly string shitItem="shit";

        public static readonly string animation = "AnimationTrigger";
        public static readonly string instantDamage = "InstantDamageTrigger";
        public static readonly string audio = "AudioTrigger";
        public static readonly string dash = "DashTrigger";
        public static readonly string bullet = "BulletTrigger";
        public static readonly string trigger2D = "Trigger2DTrigger";
        public static readonly string paraBullet = "ParaBulletTrigger";
    }
}