
using System;
using HighFive.Model.Person;

namespace HighFive.Global
{
    public class GlobalVar:ReadyGamerOne.Global.GlobalVar
    {
        public static IHighFiveCharacter G_Player { get; set; }

        /// <summary>
        /// 设置玩家
        /// </summary>
        /// <param name="player"></param>
        /// <exception cref="Exception"></exception>
        public static void SetPlayer(IHighFiveCharacter player)
        {
            if(null==G_Player)
                G_Player = player;
            else 
                throw new Exception($"已经设置G_Player[{G_Player.CharacterName}], 新来的【{player.CharacterName}】");
        }
        
        /// 外界响应灵力释放有两种方式：
        ///     一种是判断Player.IsSuper属性
        ///     一种是响应Message.InitSuper和Message.ExitSuper消息

        public static bool isSuper { get; set; }

        public static bool UsePlayerCachePos = false;
    }
}