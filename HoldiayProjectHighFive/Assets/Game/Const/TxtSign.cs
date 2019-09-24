namespace Game.Const
{
    /// <summary>
    /// 保存所有继承 ITxtSerialiable接口的类的标识
    /// 初始化的时候TxtManager类需要注册这些字符串，否则无法使用
    /// </summary>
    public static class TxtSign
    {
        public static readonly char txtSplitChar = ',';

        public static readonly string L1Rank = "L1Rank";
        public static readonly string L2Rank = "L2Rank";

        public static readonly string S1Rank = "S1Rank";
        public static readonly string S2Rank = "S2Rank";

        public static readonly string shitItem="shit";
        public static readonly string boxItem = "box";

    }
}