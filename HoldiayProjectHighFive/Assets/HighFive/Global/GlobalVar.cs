
using HighFive.Model.Person;

namespace HighFive.Global
{
    public class GlobalVar:ReadyGamerOne.Global.GlobalVar
    {
        public static IHighFiveCharacter G_Player;
        
        /// 外界响应灵力释放有两种方式：
        ///     一种是判断Player.IsSuper属性
        ///     一种是响应Message.InitSuper和Message.ExitSuper消息

        public static bool isSuper { get; set; }

        public static float superTime = 3;
    }
}